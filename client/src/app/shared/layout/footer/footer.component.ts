import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink, TranslateModule],
  template: `
    <footer class="footer">
      <div class="footer-wave">
        <svg viewBox="0 0 1440 100" preserveAspectRatio="none">
          <path d="M0,40 C360,100 720,0 1080,60 C1260,90 1380,30 1440,50 L1440,100 L0,100 Z" fill="currentColor"/>
        </svg>
      </div>
      <div class="footer-content">
        <div class="container">
          <div class="footer-grid">
            <div class="footer-brand">
              <span class="footer-logo">📚 {{ 'APP_NAME' | translate }}</span>
              <p class="footer-desc">{{ 'HOME.HERO_SUBTITLE' | translate }}</p>
              <div class="social-links">
                <a href="#" aria-label="Facebook"><i class="fab fa-facebook-f"></i></a>
                <a href="#" aria-label="YouTube"><i class="fab fa-youtube"></i></a>
                <a href="#" aria-label="Instagram"><i class="fab fa-instagram"></i></a>
              </div>
            </div>
            <div class="footer-links">
              <h4>{{ 'NAV.HOME' | translate }}</h4>
              <a routerLink="/grades">{{ 'NAV.TESTS' | translate }}</a>
              <a routerLink="/games">{{ 'NAV.GAMES' | translate }}</a>
              <a routerLink="/contact">{{ 'NAV.CONTACT' | translate }}</a>
              <a routerLink="/booking">{{ 'NAV.BOOKING' | translate }}</a>
            </div>
            <div class="footer-links">
              <h4>{{ 'GAMES.TITLE' | translate }}</h4>
              <a routerLink="/games">{{ 'GAMES.MATCHING' | translate }}</a>
              <a routerLink="/games">{{ 'GAMES.WHEEL' | translate }}</a>
              <a routerLink="/games">{{ 'GAMES.DRAGDROP' | translate }}</a>
              <a routerLink="/games">{{ 'GAMES.FLIPCARD' | translate }}</a>
            </div>
          </div>
          <div class="footer-bottom">
            <p>{{ 'COPYRIGHT' | translate }}</p>
          </div>
        </div>
      </div>
    </footer>
  `,
  styleUrl: './footer.component.scss'
})
export class FooterComponent {}
