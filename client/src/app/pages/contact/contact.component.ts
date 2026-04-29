import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <section class="contact-page">
      <div class="container">
        <div class="page-header animate-fade-in-up">
          <h1 class="section-title">{{ 'CONTACT.TITLE' | translate }}</h1>
          <p class="section-subtitle">نحن هنا لمساعدتك — تواصل معنا بأي طريقة تناسبك</p>
        </div>
        <div class="contact-grid">
          <!-- Contact Info -->
          <div class="contact-info animate-fade-in-up">
            <div class="info-card whatsapp-card">
              <div class="info-icon">💬</div>
              <h3>واتساب — أسرع طريقة للتواصل</h3>
              <p style="color:#6b7280;font-size:0.85rem;margin-bottom:0.75rem">راسلنا مباشرة عبر واتساب وسنرد عليك في أقرب وقت</p>
              <a href="https://wa.me/201000000000?text=مرحباً%20أريد%20الاستفسار%20عن%20منصة%20الإنجليزية%20مع%20ديما" target="_blank" class="btn btn-success btn-lg w-full">
                <i class="fab fa-whatsapp"></i>
                فتح واتساب
              </a>
            </div>
            <div class="info-card">
              <div class="info-icon">📧</div>
              <h3>{{ 'CONTACT.EMAIL_LABEL' | translate }}</h3>
              <a href="mailto:contact@englishwithdima.com" class="info-link">contact&#64;englishwithdima.com</a>
            </div>
          </div>

          <!-- Form -->
          <div class="contact-form-card animate-fade-in-up" style="animation-delay:.15s">
            @if (success()) {
              <div class="success-state">
                <span class="success-icon">✅</span>
                <h2>{{ 'CONTACT.SUCCESS' | translate }}</h2>
                <p>سنتواصل معك في أقرب وقت</p>
                <button class="btn btn-primary" (click)="success.set(false)">{{ 'CONTACT.SEND_ANOTHER' | translate }}</button>
              </div>
            } @else {
              <h2 class="form-title">أرسل رسالة</h2>
              <form (ngSubmit)="onSubmit()">
                <!-- Honeypot -->
                <div style="position:absolute;left:-9999px"><input type="text" [(ngModel)]="honeypot" name="website" tabindex="-1" autocomplete="off"></div>

                <div class="form-row">
                  <div class="form-group"><label for="c-name">{{ 'CONTACT.NAME' | translate }}</label><input type="text" [(ngModel)]="form.senderName" name="name" required id="c-name" placeholder="اكتب اسمك"></div>
                  <div class="form-group"><label for="c-email">{{ 'CONTACT.EMAIL' | translate }}</label><input type="email" [(ngModel)]="form.senderEmail" name="email" required id="c-email" placeholder="بريدك الإلكتروني" inputmode="email"></div>
                </div>
                <div class="form-row">
                  <div class="form-group"><label for="c-phone">{{ 'CONTACT.PHONE' | translate }}</label><input type="tel" [(ngModel)]="form.senderPhone" name="phone" id="c-phone" placeholder="01XXXXXXXXX" inputmode="tel"></div>
                  <div class="form-group"><label for="c-subject">{{ 'CONTACT.SUBJECT' | translate }}</label><input type="text" [(ngModel)]="form.subject" name="subject" required id="c-subject" placeholder="الموضوع"></div>
                </div>
                <div class="form-group"><label for="c-msg">{{ 'CONTACT.MESSAGE' | translate }}</label><textarea [(ngModel)]="form.message" name="message" rows="5" required id="c-msg" placeholder="اكتب رسالتك هنا..."></textarea></div>
                <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading()">
                  @if (loading()) {
                    <span class="btn-spinner"></span>
                  } @else {
                    <i class="fas fa-paper-plane"></i>
                    {{ 'CONTACT.SEND' | translate }}
                  }
                </button>
              </form>
            }
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .contact-page { padding:2rem 0 5rem; }
    .contact-grid { display:grid; grid-template-columns:1fr 1.5fr; gap:2rem; align-items:start;
      @media(max-width:768px){ grid-template-columns:1fr; }
    }
    .contact-form-card { background:white; border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06); position:relative; }
    .form-title { font-size:1.2rem; font-weight:700; margin-bottom:1.25rem; color:var(--gray-800); }
    .form-row { display:grid; grid-template-columns:1fr 1fr; gap:1rem; @media(max-width:600px){ grid-template-columns:1fr; } }
    .w-full { width:100%; }

    .contact-info { display:flex; flex-direction:column; gap:1rem; }
    .info-card { background:white; border-radius:16px; padding:1.5rem; box-shadow:0 2px 12px rgba(0,0,0,.04); text-align:center; transition:all .3s;
      &:hover { transform:translateY(-3px); box-shadow:0 8px 25px rgba(0,0,0,.08); }
      .info-icon { font-size:2rem; margin-bottom:.5rem; }
      h3 { font-weight:700; color:#1f2937; margin-bottom:.5rem; }
      .info-link { color:var(--primary); font-weight:600; font-size:.95rem; display:block; min-height:44px; display:flex; align-items:center; justify-content:center; }
    }
    .whatsapp-card { background:linear-gradient(135deg,#d1fae5,#ecfdf5); border:2px solid rgba(37,211,102,0.2); }

    .success-state { text-align:center; padding:3rem 2rem;
      .success-icon { font-size:4rem; display:block; margin-bottom:1rem; animation:float 2s ease-in-out infinite; }
      h2 { font-size:1.3rem; font-weight:700; color:#059669; margin-bottom:.5rem; }
      p { color:var(--gray-500); margin-bottom:1.5rem; }
    }

    .btn-spinner {
      display:inline-block; width:20px; height:20px;
      border:2.5px solid rgba(255,255,255,.3); border-top-color:white;
      border-radius:50%; animation:spin .7s linear infinite;
    }
    @keyframes spin { to { transform:rotate(360deg); } }
    @keyframes float { 0%,100%{transform:translateY(0)} 50%{transform:translateY(-8px)} }
  `]
})
export class ContactComponent {
  form = { senderName: '', senderEmail: '', senderPhone: '', subject: '', message: '' };
  honeypot = '';
  loading = signal(false);
  success = signal(false);

  constructor(private api: ApiService) {}

  onSubmit() {
    // Honeypot spam check
    if (this.honeypot) return;

    this.loading.set(true);
    this.api.sendContactMessage(this.form).subscribe({
      next: () => { this.loading.set(false); this.success.set(true); this.form = { senderName: '', senderEmail: '', senderPhone: '', subject: '', message: '' }; },
      error: () => this.loading.set(false)
    });
  }
}
