import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { SeoService } from '../../core/services/seo.service';
import { Grade } from '../../core/models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  grades = signal<Grade[]>([]);

  // Modal state
  showChoiceModal = false;
  modalStep: 'choose' | 'guest' = 'choose';

  // Guest form state
  guestName = '';
  guestGradeId: number | null = null;
  guestError = '';
  guestApiError = '';
  guestLoading = false;

  games = [
    { icon: '🔗', key: 'MATCHING', route: '/games', color: '#6366f1' },
    { icon: '🎡', key: 'WHEEL', route: '/games', color: '#a855f7' },
    { icon: '✋', key: 'DRAGDROP', route: '/games', color: '#06b6d4' },
    { icon: '🃏', key: 'FLIPCARD', route: '/games', color: '#10b981' },
  ];

  features = [
    { icon: '📝', key: 'FEATURE_TESTS', descKey: 'FEATURE_TESTS_DESC', color: '#6366f1' },
    { icon: '🎮', key: 'FEATURE_GAMES', descKey: 'FEATURE_GAMES_DESC', color: '#a855f7' },
    { icon: '📊', key: 'FEATURE_PROGRESS', descKey: 'FEATURE_PROGRESS_DESC', color: '#06b6d4' },
    { icon: '👩‍🏫', key: 'FEATURE_TEACHER', descKey: 'FEATURE_TEACHER_DESC', color: '#f59e0b' },
  ];

  constructor(
    private api: ApiService,
    private auth: AuthService,
    private router: Router,
    private seo: SeoService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.seo.setPage({
      titleDefault: 'الرئيسية',
      descriptionDefault: 'الإنجليزية مع ديما — تعلم الإنجليزية من خلال اختبارات تفاعلية وألعاب تعليمية. منصة لطلاب المرحلة الابتدائية والإعدادية في مصر.',
      keywords: 'تعلم الإنجليزية, الإنجليزية مع ديما, طلاب مصر, اختبارات إنجليزي, ألعاب إنجليزي, إنجليزي ابتدائي, إنجليزي إعدادي'
    });

    this.api.getGrades().subscribe(res => {
      if (res.success) this.grades.set(res.data);
    });
  }

  getGradeEmoji(level: number): string {
    const emojis = ['1️⃣', '2️⃣', '3️⃣', '4️⃣', '5️⃣', '6️⃣', '7️⃣', '8️⃣', '9️⃣'];
    return emojis[level - 1] || '📚';
  }

  openChoiceModal(): void {
    // If already logged in or guest, go straight to grades
    if (this.auth.isLoggedIn() || this.auth.isGuest()) {
      this.router.navigate(['/grades']);
      return;
    }
    this.showChoiceModal = true;
    this.modalStep = 'choose';
    this.guestName = '';
    this.guestGradeId = null;
    this.guestError = '';
    this.guestApiError = '';
  }

  closeModal(event: MouseEvent): void {
    // Close only if clicking the overlay background
    if ((event.target as HTMLElement).id === 'choice-modal-overlay') {
      this.showChoiceModal = false;
    }
  }

  onGuestSubmit(): void {
    // Validate name
    const trimmedName = this.guestName.trim();
    if (!trimmedName) {
      this.guestError = this.translate.instant('HOME.GUEST_NAME_REQUIRED');
      return;
    }
    if (trimmedName.length < 2) {
      this.guestError = this.translate.instant('HOME.GUEST_NAME_MIN');
      return;
    }

    this.guestError = '';
    this.guestApiError = '';
    this.guestLoading = true;

    // If no grade selected, store guest locally and navigate
    const gradeId = this.guestGradeId || 0;

    // Try the API guest session first
    this.auth.guestSession({ displayName: trimmedName, gradeId }).subscribe({
      next: res => {
        this.guestLoading = false;
        if (res.success) {
          this.auth.handleGuestSession(res.data);
          this.showChoiceModal = false;
          this.router.navigate(['/grades']);
        } else {
          // API returned but not success — fall back to local guest
          this.auth.setLocalGuest(trimmedName, gradeId);
          this.showChoiceModal = false;
          this.router.navigate(['/grades']);
        }
      },
      error: () => {
        // API unreachable — fall back to local guest
        this.guestLoading = false;
        this.auth.setLocalGuest(trimmedName, gradeId);
        this.showChoiceModal = false;
        this.router.navigate(['/grades']);
      }
    });
  }
}
