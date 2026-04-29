import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../core/services/api.service';
import { SeoService } from '../../core/services/seo.service';
import { Grade } from '../../core/models';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <section class="booking-page">
      <div class="container">
        <div class="page-header animate-fade-in-up">
          <h1 class="section-title">{{ 'CONTACT.BOOKING_TITLE' | translate }}</h1>
          <p class="section-subtitle">{{ 'BOOKING.SUBTITLE' | translate }}</p>
        </div>
        <div class="booking-grid">
          <div class="booking-form-card animate-fade-in-up">
            @if (success()) {
              <div class="success-state">
                <span>🎉</span>
                <h2>{{ 'BOOKING.SENT_TITLE' | translate }}</h2>
                <p>{{ 'BOOKING.SENT_DESC' | translate }}</p>
                <button class="btn btn-primary" (click)="success.set(false)">{{ 'BOOKING.BOOK_ANOTHER' | translate }}</button>
              </div>
            } @else {
              <form (ngSubmit)="onSubmit()">
                <div class="form-row">
                  <div class="form-group"><label>{{ 'CONTACT.PARENT_NAME' | translate }}</label><input type="text" [(ngModel)]="form.parentName" name="parentName" required></div>
                  <div class="form-group"><label>{{ 'CONTACT.STUDENT_NAME' | translate }}</label><input type="text" [(ngModel)]="form.studentName" name="studentName" required></div>
                </div>
                <div class="form-row">
                  <div class="form-group"><label>{{ 'CONTACT.EMAIL' | translate }}</label><input type="email" [(ngModel)]="form.email" name="email" required></div>
                  <div class="form-group"><label>{{ 'CONTACT.PHONE' | translate }}</label><input type="tel" [(ngModel)]="form.phone" name="phone" required></div>
                </div>
                <div class="form-group">
                  <label>{{ 'AUTH.GRADE' | translate }}</label>
                  <select [(ngModel)]="form.gradeId" name="gradeId" required>
                    @for (grade of grades(); track grade.id) {
                      <option [ngValue]="grade.id">{{ grade.nameAr }}</option>
                    }
                  </select>
                </div>
                <div class="form-group"><label>{{ 'CONTACT.PREFERRED_DATES' | translate }}</label><input type="text" [(ngModel)]="form.preferredDates" name="dates" [placeholder]="'BOOKING.DATES_PLACEHOLDER' | translate"></div>
                <div class="form-group"><label>{{ 'CONTACT.MESSAGE' | translate }}</label><textarea [(ngModel)]="form.message" name="message" rows="3"></textarea></div>
                @if (error()) {
                  <div class="error-msg" style="background:#fef2f2;color:#dc2626;padding:.7rem 1rem;border-radius:10px;font-size:.9rem;margin-bottom:1rem;text-align:center">⚠️ حدث خطأ في الإرسال — حاول مجدداً</div>
                }
                <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading()">{{ 'CONTACT.SUBMIT_BOOKING' | translate }}</button>
              </form>
            }
          </div>
          <div class="booking-info animate-slide-right">
            <div class="info-block">
              <h3>{{ 'BOOKING.WHAT_TO_EXPECT' | translate }}</h3>
              <ul>
                <li>{{ 'BOOKING.EXPECT_1' | translate }}</li>
                <li>{{ 'BOOKING.EXPECT_2' | translate }}</li>
                <li>{{ 'BOOKING.EXPECT_3' | translate }}</li>
                <li>{{ 'BOOKING.EXPECT_4' | translate }}</li>
                <li>{{ 'BOOKING.EXPECT_5' | translate }}</li>
              </ul>
            </div>
            <div class="info-block highlight">
              <h3>{{ 'BOOKING.PRICING_TITLE' | translate }}</h3>
              <p class="price" [innerHTML]="'BOOKING.PRICE_FROM' | translate"></p>
              <p class="price-note">{{ 'BOOKING.GROUP_DISCOUNT' | translate }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .booking-page { padding:2rem 0 5rem; }
    .booking-grid { display:grid; grid-template-columns:1.5fr 1fr; gap:2rem; @media(max-width:768px){ grid-template-columns:1fr; } }
    .booking-form-card { background:white; border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06); }
    .form-row { display:grid; grid-template-columns:1fr 1fr; gap:1rem; @media(max-width:480px){ grid-template-columns:1fr; } }
    .w-full { width:100%; }
    .booking-info { display:flex; flex-direction:column; gap:1.5rem; }
    .info-block { background:white; border-radius:16px; padding:1.5rem; box-shadow:0 2px 12px rgba(0,0,0,.04);
      h3 { font-size:1.1rem; font-weight:700; margin-bottom:1rem; color:#1f2937; }
      ul { list-style:none; li { padding:.4rem 0; font-size:.95rem; color:#4b5563; } }
      &.highlight { background:linear-gradient(135deg,#f5f3ff,#ede9fe); border:2px solid #c4b5fd; }
      .price { font-size:1.2rem; color:#6366f1; strong { font-size:1.6rem; font-weight:900; } }
      .price-note { font-size:.85rem; color:#6b7280; margin-top:.25rem; }
    }
    .success-state { text-align:center; padding:3rem 2rem; span { font-size:4rem; display:block; margin-bottom:1rem; } h2 { font-size:1.3rem; font-weight:700; color:#059669; margin-bottom:.5rem; } p { color:#6b7280; margin-bottom:1.5rem; } }
  `]
})
export class BookingComponent implements OnInit {
  form: any = { parentName: '', studentName: '', email: '', phone: '', gradeId: null, preferredDates: '', message: '' };
  grades = signal<Grade[]>([]);
  loading = signal(false);
  success = signal(false);
  error = signal(false);

  constructor(private api: ApiService, private seo: SeoService) {}

  ngOnInit() {
    this.seo.setPage({
      titleDefault: 'حجز حصة',
      descriptionDefault: 'احجز حصة خاصة مع المعلمة ديما — دروس إنجليزي مخصصة لمستوى طفلك',
      keywords: 'حجز حصة, دروس خصوصية, إنجليزي, المعلمة ديما, حصة إنجليزي'
    });
    this.api.getGrades().subscribe(res => { if (res.success) this.grades.set(res.data); });
  }

  onSubmit() {
    this.loading.set(true);
    this.error.set(false);
    this.api.sendBookingRequest(this.form).subscribe({
      next: () => { this.loading.set(false); this.success.set(true); this.form = { parentName: '', studentName: '', email: '', phone: '', gradeId: null, preferredDates: '', message: '' }; },
      error: () => { this.loading.set(false); this.error.set(true); }
    });
  }
}
