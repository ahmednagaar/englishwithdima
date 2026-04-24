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
        </div>
        <div class="contact-grid">
          <div class="contact-form-card animate-fade-in-up">
            @if (success()) {
              <div class="success-state">
                <span class="success-icon">✅</span>
                <h2>{{ 'CONTACT.SUCCESS' | translate }}</h2>
                <button class="btn btn-primary" (click)="success.set(false)">{{ 'CONTACT.SEND_ANOTHER' | translate }}</button>
              </div>
            } @else {
              <form (ngSubmit)="onSubmit()">
                <div class="form-group"><label>{{ 'CONTACT.NAME' | translate }}</label><input type="text" [(ngModel)]="form.senderName" name="name" required></div>
                <div class="form-group"><label>{{ 'CONTACT.EMAIL' | translate }}</label><input type="email" [(ngModel)]="form.senderEmail" name="email" required></div>
                <div class="form-group"><label>{{ 'CONTACT.PHONE' | translate }}</label><input type="tel" [(ngModel)]="form.senderPhone" name="phone"></div>
                <div class="form-group"><label>{{ 'CONTACT.SUBJECT' | translate }}</label><input type="text" [(ngModel)]="form.subject" name="subject" required></div>
                <div class="form-group"><label>{{ 'CONTACT.MESSAGE' | translate }}</label><textarea [(ngModel)]="form.message" name="message" rows="5" required></textarea></div>
                <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading()">{{ 'CONTACT.SEND' | translate }}</button>
              </form>
            }
          </div>
          <div class="contact-info animate-slide-right">
            <div class="info-card">
              <div class="info-icon">📧</div>
              <h3>{{ 'CONTACT.EMAIL_LABEL' | translate }}</h3>
              <p>contact&#64;englishwithdima.com</p>
            </div>
            <div class="info-card">
              <div class="info-icon">📱</div>
              <h3>{{ 'CONTACT.PHONE_LABEL' | translate }}</h3>
              <p dir="ltr">+20 100 000 0000</p>
            </div>
            <div class="info-card whatsapp-card">
              <div class="info-icon">💬</div>
              <h3>{{ 'CONTACT.WHATSAPP' | translate }}</h3>
              <a href="https://wa.me/201000000000" target="_blank" class="btn btn-success">{{ 'CONTACT.WHATSAPP' | translate }}</a>
            </div>
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    .contact-page { padding:2rem 0 5rem; }
    .contact-grid { display:grid; grid-template-columns:1.5fr 1fr; gap:2rem; @media(max-width:768px){ grid-template-columns:1fr; } }
    .contact-form-card { background:white; border-radius:20px; padding:2rem; box-shadow:0 4px 20px rgba(0,0,0,.06); }
    .w-full { width:100%; }
    .contact-info { display:flex; flex-direction:column; gap:1rem; }
    .info-card { background:white; border-radius:16px; padding:1.5rem; box-shadow:0 2px 12px rgba(0,0,0,.04); text-align:center; transition:all .3s;
      &:hover { transform:translateY(-3px); box-shadow:0 8px 25px rgba(0,0,0,.08); }
      .info-icon { font-size:2rem; margin-bottom:.5rem; }
      h3 { font-weight:700; color:#1f2937; margin-bottom:.25rem; }
      p { color:#6b7280; font-size:.9rem; }
    }
    .whatsapp-card { background:linear-gradient(135deg,#d1fae5,#ecfdf5); }
    .success-state { text-align:center; padding:3rem 2rem; .success-icon { font-size:4rem; display:block; margin-bottom:1rem; } h2 { font-size:1.3rem; font-weight:700; color:#059669; margin-bottom:1.5rem; } }
  `]
})
export class ContactComponent {
  form = { senderName: '', senderEmail: '', senderPhone: '', subject: '', message: '' };
  loading = signal(false);
  success = signal(false);

  constructor(private api: ApiService) {}

  onSubmit() {
    this.loading.set(true);
    this.api.sendContactMessage(this.form).subscribe({
      next: () => { this.loading.set(false); this.success.set(true); this.form = { senderName: '', senderEmail: '', senderPhone: '', subject: '', message: '' }; },
      error: () => this.loading.set(false)
    });
  }
}
