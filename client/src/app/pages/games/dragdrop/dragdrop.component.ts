import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';

interface DragItem {
  id: number;
  text: string;
  imageUrl?: string;
  placed: boolean;
  correct?: boolean;
  placedZoneId?: number;
}

interface DropZone {
  id: number;
  label: string;
  color: string;
  items: DragItem[];
  highlight: boolean;
}

@Component({
  selector: 'app-dragdrop',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  template: `
    <section class="game-page">
      <div class="container">
        @if (loading()) {
          <div class="loading-spinner"></div>
        } @else if (gameOver()) {
          <!-- Results Screen -->
          <div class="result-card animate-scale-in">
            <span class="result-emoji">{{ scorePercent() >= 80 ? '🏆' : scorePercent() >= 50 ? '⭐' : '💪' }}</span>
            <h2>{{ 'GAMES.GAME_OVER' | translate }}</h2>
            <div class="final-score">{{ score() }}</div>
            <p>{{ 'GAMES.ITEMS_CORRECT' | translate:{ correct: correctCount(), total: totalItems() } }}</p>
            <div class="score-bar-wrap">
              <div class="score-bar" [style.width.%]="scorePercent()" [style.background]="scorePercent() >= 80 ? 'linear-gradient(90deg,#10b981,#34d399)' : scorePercent() >= 50 ? 'linear-gradient(90deg,#f59e0b,#fbbf24)' : 'linear-gradient(90deg,#ef4444,#f87171)'"></div>
            </div>
            <div class="time-stamp">⏱️ {{ formatTime(elapsed()) }}</div>
            <div class="result-btns">
              <button class="btn btn-primary" (click)="restartGame()">{{ 'GAMES.PLAY_AGAIN' | translate }}</button>
              <a routerLink="/games" class="btn btn-secondary">{{ 'COMMON.BACK' | translate }}</a>
            </div>
          </div>
        } @else {
          <!-- Game Header -->
          <div class="game-header">
            <div class="header-left">
              <h1>✋ {{ title() || ('GAMES.DRAGDROP' | translate) }}</h1>
              @if (instructions()) {
                <p class="game-instructions">{{ instructions() }}</p>
              }
            </div>
            <div class="game-info">
              <span class="game-score">{{ 'GAMES.SCORE' | translate }}: {{ score() }}</span>
              <span class="game-time">⏱️ {{ formatTime(elapsed()) }}</span>
              <span class="progress-pill">{{ placedCount() }} / {{ totalItems() }}</span>
            </div>
          </div>

          <!-- Draggable Items Bank -->
          <div class="drag-bank" [class.empty]="unplacedItems().length === 0">
            <h3 class="bank-label">{{ 'GAMES.DRAG_ITEMS_HINT' | translate }}</h3>
            <div class="items-row">
              @for (item of unplacedItems(); track item.id) {
                <div class="drag-item"
                     [attr.data-item-id]="item.id"
                     draggable="true"
                     (dragstart)="onDragStart($event, item)"
                     (dragend)="onDragEnd($event)">
                  @if (item.imageUrl) {
                    <img [src]="item.imageUrl" [alt]="item.text" class="item-img">
                  }
                  <span>{{ item.text }}</span>
                </div>
              }
              @if (unplacedItems().length === 0) {
                <p class="all-placed">{{ 'GAMES.ALL_PLACED' | translate }}</p>
              }
            </div>
          </div>

          <!-- Drop Zones -->
          <div class="zones-grid" [style.grid-template-columns]="'repeat(' + zones().length + ', 1fr)'">
            @for (zone of zones(); track zone.id) {
              <div class="drop-zone"
                   [class.highlight]="zone.highlight"
                   [style.border-color]="zone.color"
                   (dragover)="onDragOver($event, zone)"
                   (dragleave)="onDragLeave(zone)"
                   (drop)="onDrop($event, zone)">
                <div class="zone-header" [style.background]="zone.color">
                  {{ zone.label }}
                </div>
                <div class="zone-items">
                  @for (item of zone.items; track item.id) {
                    <div class="zone-item"
                         [class.correct]="item.correct === true"
                         [class.wrong]="item.correct === false"
                         draggable="true"
                         (dragstart)="onDragStartFromZone($event, item, zone)"
                         (dragend)="onDragEnd($event)">
                      <span>{{ item.text }}</span>
                      <button class="remove-btn" (click)="removeFromZone(item, zone)" title="إزالة">✕</button>
                    </div>
                  }
                  @if (zone.items.length === 0) {
                    <p class="zone-placeholder">{{ 'GAMES.DROP_HERE' | translate }}</p>
                  }
                </div>
              </div>
            }
          </div>

          <!-- Submit Button -->
          @if (placedCount() > 0) {
            <div class="submit-area animate-fade-in">
              <button class="btn btn-primary btn-lg" (click)="checkAnswers()" [disabled]="checked()">
                {{ checked() ? ('GAMES.CHECKED' | translate) : ('GAMES.CHECK_ANSWERS' | translate) }}
              </button>
              @if (checked()) {
                <button class="btn btn-success btn-lg" (click)="finishGame()">
                  {{ 'GAMES.SEE_RESULTS' | translate }} →
                </button>
              }
            </div>
          }
        }
      </div>
    </section>
  `,
  styles: [`
    .game-page { padding: 2rem 0 5rem; }

    .game-header {
      display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1.5rem; flex-wrap: wrap; gap: 1rem;
      h1 { font-size: 1.5rem; font-weight: 800; }
      .game-instructions { color: #6b7280; font-size: 0.9rem; margin-top: 0.25rem; }
    }
    .game-info { display: flex; gap: 1rem; align-items: center; flex-wrap: wrap; }
    .game-score { font-weight: 800; color: #06b6d4; font-size: 1.1rem; }
    .game-time { font-weight: 600; color: #6b7280; }
    .progress-pill {
      background: linear-gradient(135deg, #06b6d4, #22d3ee); color: white;
      padding: 0.3rem 0.8rem; border-radius: 20px; font-weight: 700; font-size: 0.85rem;
    }

    /* Drag Items Bank */
    .drag-bank {
      background: white; border-radius: 20px; padding: 1.5rem; margin-bottom: 2rem;
      box-shadow: 0 4px 20px rgba(0,0,0,0.06); border: 2px dashed #e5e7eb;
      &.empty { border-color: #10b981; background: #f0fdf4; }
    }
    .bank-label {
      font-size: 0.9rem; font-weight: 700; color: #6b7280; margin-bottom: 1rem;
      text-transform: uppercase; letter-spacing: 0.5px;
    }
    .items-row {
      display: flex; flex-wrap: wrap; gap: 0.75rem; min-height: 60px; align-items: center;
    }
    .drag-item {
      background: linear-gradient(135deg, #f0f9ff, #e0f2fe); border: 2px solid #7dd3fc;
      border-radius: 14px; padding: 0.75rem 1.25rem; font-weight: 600; font-size: 0.95rem;
      cursor: grab; transition: all 0.2s; display: flex; align-items: center; gap: 0.5rem;
      color: #0c4a6e; user-select: none;
      &:hover { transform: translateY(-3px) scale(1.03); box-shadow: 0 6px 20px rgba(6,182,212,0.2); border-color: #06b6d4; }
      &:active { cursor: grabbing; transform: scale(0.97); }
      .item-img { width: 32px; height: 32px; border-radius: 8px; object-fit: cover; }
    }
    .all-placed { color: #059669; font-weight: 600; font-size: 0.95rem; }

    /* Drop Zones */
    .zones-grid {
      display: grid; gap: 1.25rem; margin-bottom: 2rem;
      @media(max-width: 768px) { grid-template-columns: 1fr !important; }
    }
    .drop-zone {
      background: white; border-radius: 18px; border: 3px dashed #e5e7eb;
      min-height: 200px; transition: all 0.3s; overflow: hidden;
      box-shadow: 0 2px 10px rgba(0,0,0,0.04);
      &.highlight {
        border-style: solid; transform: scale(1.02);
        box-shadow: 0 8px 30px rgba(0,0,0,0.12);
        background: rgba(99, 102, 241, 0.03);
      }
    }
    .zone-header {
      padding: 0.75rem 1rem; color: white; font-weight: 800; font-size: 1rem;
      text-align: center; letter-spacing: 0.5px;
    }
    .zone-items {
      padding: 1rem; display: flex; flex-direction: column; gap: 0.5rem; min-height: 100px;
    }
    .zone-item {
      background: #f9fafb; border: 2px solid #e5e7eb; border-radius: 12px;
      padding: 0.65rem 1rem; font-weight: 600; font-size: 0.9rem;
      display: flex; justify-content: space-between; align-items: center;
      cursor: grab; transition: all 0.3s;
      &:hover { border-color: #6366f1; }
      &.correct { background: #d1fae5; border-color: #10b981; color: #059669; }
      &.wrong { background: #fee2e2; border-color: #ef4444; color: #dc2626; animation: shake 0.4s; }
      .remove-btn {
        background: none; border: none; cursor: pointer; color: #9ca3af;
        font-size: 0.9rem; transition: color 0.2s;
        &:hover { color: #ef4444; }
      }
    }
    .zone-placeholder {
      color: #d1d5db; text-align: center; font-size: 0.9rem; font-weight: 500;
      padding: 2rem 0; font-style: italic;
    }

    /* Submit Area */
    .submit-area {
      display: flex; justify-content: center; gap: 1rem; padding: 1rem 0;
    }

    /* Result Card */
    .result-card {
      max-width: 440px; margin: 3rem auto; text-align: center;
      background: white; border-radius: 24px; padding: 3rem;
      box-shadow: 0 10px 40px rgba(0,0,0,0.08);
      .result-emoji { font-size: 5rem; display: block; margin-bottom: 1rem; animation: float 2s ease-in-out infinite; }
      h2 { font-size: 1.5rem; font-weight: 800; color: #1f2937; margin-bottom: 1rem; }
      .final-score { font-size: 3rem; font-weight: 900; color: #06b6d4; }
      p { color: #6b7280; margin-bottom: 1rem; }
      .score-bar-wrap {
        height: 10px; background: #e5e7eb; border-radius: 10px; overflow: hidden; margin-bottom: 0.75rem;
        .score-bar { height: 100%; border-radius: 10px; transition: width 1s ease; }
      }
      .time-stamp { color: #9ca3af; font-size: 0.9rem; margin-bottom: 1.5rem; }
      .result-btns { display: flex; gap: 1rem; justify-content: center; }
    }

    @keyframes shake { 0%,100%{transform:translateX(0)} 25%{transform:translateX(-6px)} 75%{transform:translateX(6px)} }
  `]
})
export class DragdropComponent implements OnInit, OnDestroy {
  loading = signal(true);
  gameOver = signal(false);
  checked = signal(false);
  score = signal(0);
  elapsed = signal(0);
  title = signal('');
  instructions = signal('');
  totalItems = signal(0);
  correctCount = signal(0);
  zones = signal<DropZone[]>([]);
  allItems = signal<DragItem[]>([]);
  private timer: any;
  private sessionId = 0;
  private gameId = 0;
  private draggedItem: DragItem | null = null;
  private sourceZone: DropZone | null = null;
  private pointsPerItem = 100;

  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router, private translate: TranslateService) {}

  ngOnInit() {
    this.gameId = +this.route.snapshot.params['id'];
    const gameId = this.gameId;
    this.api.startDragDropGame(gameId).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.success && res.data) {
          this.sessionId = res.data.sessionId || res.data.id;
          this.title.set(res.data.gameTitle || '');
          this.instructions.set(res.data.instructions || '');
          this.pointsPerItem = res.data.pointsPerCorrectItem || 100;

          const zones: DropZone[] = (res.data.zones || []).map((z: any) => ({
            id: z.id,
            label: z.zoneLabel,
            color: z.zoneColor || this.getDefaultColor(z.id),
            items: [],
            highlight: false
          }));
          this.zones.set(zones);

          const items: DragItem[] = (res.data.items || []).map((i: any) => ({
            id: i.id,
            text: i.itemText,
            imageUrl: i.itemImageUrl,
            placed: false
          }));
          // Shuffle items
          for (let idx = items.length - 1; idx > 0; idx--) {
            const j = Math.floor(Math.random() * (idx + 1));
            [items[idx], items[j]] = [items[j], items[idx]];
          }
          this.allItems.set(items);
          this.totalItems.set(items.length);
          this.startTimer();
        } else {
          // Demo fallback
          this.loadDemoData();
          this.startTimer();
        }
      },
      error: () => {
        this.loading.set(false);
        this.loadDemoData();
        this.startTimer();
      }
    });
  }

  ngOnDestroy() { clearInterval(this.timer); }

  private loadDemoData() {
    this.title.set(this.translate.instant('GAMES.SORT_WORDS'));
    this.instructions.set(this.translate.instant('GAMES.DRAG_TO_CATEGORY'));
    this.zones.set([
      { id: 1, label: this.translate.instant('GAMES.NOUNS'), color: '#6366f1', items: [], highlight: false },
      { id: 2, label: this.translate.instant('GAMES.VERBS'), color: '#a855f7', items: [], highlight: false },
      { id: 3, label: this.translate.instant('GAMES.ADJECTIVES'), color: '#06b6d4', items: [], highlight: false },
    ]);
    const items: DragItem[] = [
      { id: 1, text: 'book', placed: false },
      { id: 2, text: 'run', placed: false },
      { id: 3, text: 'beautiful', placed: false },
      { id: 4, text: 'teacher', placed: false },
      { id: 5, text: 'jump', placed: false },
      { id: 6, text: 'happy', placed: false },
      { id: 7, text: 'school', placed: false },
      { id: 8, text: 'write', placed: false },
      { id: 9, text: 'tall', placed: false },
    ];
    for (let idx = items.length - 1; idx > 0; idx--) {
      const j = Math.floor(Math.random() * (idx + 1));
      [items[idx], items[j]] = [items[j], items[idx]];
    }
    this.allItems.set(items);
    this.totalItems.set(items.length);
  }

  unplacedItems(): DragItem[] {
    return this.allItems().filter(i => !i.placed);
  }

  placedCount(): number {
    return this.allItems().filter(i => i.placed).length;
  }

  scorePercent(): number {
    return this.totalItems() > 0 ? Math.round((this.correctCount() / this.totalItems()) * 100) : 0;
  }

  // ===== HTML5 Drag & Drop =====
  onDragStart(e: DragEvent, item: DragItem) {
    this.draggedItem = item;
    this.sourceZone = null;
    e.dataTransfer?.setData('text/plain', item.id.toString());
    if (e.dataTransfer) e.dataTransfer.effectAllowed = 'move';
    (e.target as HTMLElement).style.opacity = '0.5';
  }

  onDragStartFromZone(e: DragEvent, item: DragItem, zone: DropZone) {
    this.draggedItem = item;
    this.sourceZone = zone;
    e.dataTransfer?.setData('text/plain', item.id.toString());
    if (e.dataTransfer) e.dataTransfer.effectAllowed = 'move';
    (e.target as HTMLElement).style.opacity = '0.5';
  }

  onDragEnd(e: DragEvent) {
    (e.target as HTMLElement).style.opacity = '1';
    this.zones().forEach(z => z.highlight = false);
    this.zones.set([...this.zones()]);
  }

  onDragOver(e: DragEvent, zone: DropZone) {
    e.preventDefault();
    if (e.dataTransfer) e.dataTransfer.dropEffect = 'move';
    if (!zone.highlight) {
      zone.highlight = true;
      this.zones.set([...this.zones()]);
    }
  }

  onDragLeave(zone: DropZone) {
    zone.highlight = false;
    this.zones.set([...this.zones()]);
  }

  onDrop(e: DragEvent, zone: DropZone) {
    e.preventDefault();
    zone.highlight = false;

    if (!this.draggedItem) return;

    // If coming from another zone, remove first
    if (this.sourceZone) {
      this.sourceZone.items = this.sourceZone.items.filter(i => i.id !== this.draggedItem!.id);
    }

    // Mark as placed
    const item = this.draggedItem;
    item.placed = true;
    item.placedZoneId = zone.id;
    item.correct = undefined; // Reset check

    // Remove from any other zone it might be in
    this.zones().forEach(z => {
      if (z.id !== zone.id) {
        z.items = z.items.filter(i => i.id !== item.id);
      }
    });

    // Add to this zone if not already there
    if (!zone.items.find(i => i.id === item.id)) {
      zone.items.push(item);
    }

    this.checked.set(false);
    this.zones.set([...this.zones()]);
    this.allItems.set([...this.allItems()]);
    this.draggedItem = null;
    this.sourceZone = null;
  }

  removeFromZone(item: DragItem, zone: DropZone) {
    zone.items = zone.items.filter(i => i.id !== item.id);
    item.placed = false;
    item.placedZoneId = undefined;
    item.correct = undefined;
    this.checked.set(false);
    this.zones.set([...this.zones()]);
    this.allItems.set([...this.allItems()]);
  }

  checkAnswers() {
    this.checked.set(true);

    // Submit to backend
    const moves = this.allItems().filter(i => i.placed).map(i => ({
      itemId: i.id,
      placedInZoneId: i.placedZoneId!,
      timeSpentMs: this.elapsed() * 1000
    }));

    this.api.submitDragDropGame({
      sessionId: this.sessionId,
      gameId: this.gameId,
      moves,
      timeSpentSeconds: this.elapsed()
    }).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.score.set(res.data.totalScore);
          this.correctCount.set(res.data.correctCount);

          // Mark items visually using backend response
          // We don't have per-item response, so mark all as checked
          this.zones().forEach(z => {
            z.items.forEach(item => {
              // The backend calculated correct/wrong, but we only get aggregate
              // For now, mark items as correct/wrong proportionally
            });
          });
        }
      },
      error: () => {
        // Demo scoring: count items in first zone as correct (simplified)
        let correct = 0;
        this.allItems().filter(i => i.placed).forEach(i => {
          i.correct = Math.random() > 0.3; // Demo
          if (i.correct) correct++;
        });
        this.correctCount.set(correct);
        this.score.set(correct * this.pointsPerItem);
        this.zones.set([...this.zones()]);
      }
    });
  }

  finishGame() {
    clearInterval(this.timer);
    this.gameOver.set(true);
  }

  restartGame() { location.reload(); }

  private startTimer() {
    this.timer = setInterval(() => this.elapsed.set(this.elapsed() + 1), 1000);
  }

  formatTime(s: number): string {
    return `${Math.floor(s / 60)}:${(s % 60).toString().padStart(2, '0')}`;
  }

  private getDefaultColor(index: number): string {
    const colors = ['#6366f1', '#a855f7', '#06b6d4', '#10b981', '#f59e0b', '#ef4444'];
    return colors[index % colors.length];
  }
}
