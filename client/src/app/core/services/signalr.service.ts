import { Injectable, OnDestroy, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export interface RealTimeNotification {
  title: string;
  body: string;
  timestamp: Date;
}

export interface BadgeEarnedEvent {
  badgeName: string;
  badgeIcon: string;
  bonusPoints: number;
  timestamp: Date;
}

export interface LeaderboardUpdateEvent {
  userId: string;
  displayName: string;
  totalPoints: number;
  rank: number;
  gradeId: number;
  timestamp: Date;
}

export interface TestCompletedEvent {
  testId: number;
  testTitle: string;
  score: number;
  maxScore: number;
  percentage: number;
  timestamp: Date;
}

@Injectable({ providedIn: 'root' })
export class SignalRService implements OnDestroy {
  private connection: signalR.HubConnection | null = null;

  // Reactive signals for real-time events
  notifications = signal<RealTimeNotification[]>([]);
  latestBadge = signal<BadgeEarnedEvent | null>(null);
  leaderboardUpdates = signal<LeaderboardUpdateEvent[]>([]);
  connected = signal(false);

  // Toast queue for visual notifications
  toasts = signal<{ id: number; message: string; type: string; icon: string }[]>([]);
  private toastId = 0;

  constructor(private auth: AuthService) {}

  async start(gradeId?: number): Promise<void> {
    if (this.connection) return;

    const token = this.auth.getToken();
    let url = `${environment.apiUrl.replace('/api', '')}/hubs/notifications`;
    if (gradeId) url += `?gradeId=${gradeId}`;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(url, {
        accessTokenFactory: () => token || ''
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    // Register event handlers
    this.connection.on('Notification', (data: RealTimeNotification) => {
      this.notifications.set([data, ...this.notifications().slice(0, 49)]);
      this.showToast(data.title, 'info', '🔔');
    });

    this.connection.on('BadgeEarned', (data: BadgeEarnedEvent) => {
      this.latestBadge.set(data);
      this.showToast(`${data.badgeIcon} Badge earned: ${data.badgeName}! +${data.bonusPoints}pts`, 'success', '🏆');
    });

    this.connection.on('LeaderboardUpdate', (data: LeaderboardUpdateEvent) => {
      const updates = this.leaderboardUpdates();
      const idx = updates.findIndex(u => u.userId === data.userId);
      if (idx >= 0) updates[idx] = data;
      else updates.push(data);
      this.leaderboardUpdates.set([...updates]);
    });

    this.connection.on('TestCompleted', (data: TestCompletedEvent) => {
      this.showToast(`Test "${data.testTitle}" completed: ${data.percentage}%`, data.percentage >= 70 ? 'success' : 'warning', '📝');
    });

    this.connection.onreconnecting(() => this.connected.set(false));
    this.connection.onreconnected(() => this.connected.set(true));
    this.connection.onclose(() => this.connected.set(false));

    try {
      await this.connection.start();
      this.connected.set(true);
    } catch (err) {
      console.warn('SignalR connection failed:', err);
    }
  }

  async joinLeaderboard(gradeId: number): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('JoinGradeLeaderboard', gradeId);
    }
  }

  async leaveLeaderboard(gradeId: number): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('LeaveGradeLeaderboard', gradeId);
    }
  }

  private showToast(message: string, type: string, icon: string) {
    const id = ++this.toastId;
    const toast = { id, message, type, icon };
    this.toasts.set([...this.toasts(), toast]);

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
      this.toasts.set(this.toasts().filter(t => t.id !== id));
    }, 5000);
  }

  dismissToast(id: number) {
    this.toasts.set(this.toasts().filter(t => t.id !== id));
  }

  async stop(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.connected.set(false);
    }
  }

  ngOnDestroy() {
    this.stop();
  }
}
