import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';

interface Card {
  id: number;
  pairId: number;
  text: string;
  imageUrl?: string;
  audioUrl?: string;
  flipped: boolean;
  matched: boolean;
  mismatch: boolean;
}

@Component({
  selector: 'app-flipcard',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="game-page">
      <div class="container">
        @if (loading()) {
          <div class="loading-spinner"></div>
        } @else if (gameOver()) {
          <div class="result-card animate-scale-in">
            <span class="result-emoji">{{ scorePercent() >= 80 ? '🏆' : scorePercent() >= 50 ? '⭐' : '💪' }}</span>
            <h2>{{ 'GAMES.GAME_OVER' | translate }}</h2>
            <div class="final-score">{{ score() }}</div>
            <p>{{ 'GAMES.MATCHED_COUNT' | translate:{ matched: matchedCount(), total: totalPairs() } }}</p>

            <div class="stats-row">
              <div class="stat-box">
                <span class="stat-value">{{ totalFlips() }}</span>
                <span class="stat-label">{{ 'GAMES.TOTAL_FLIPS' | translate }}</span>
              </div>
              <div class="stat-box">
                <span class="stat-value">{{ wrongFlips() }}</span>
                <span class="stat-label">{{ 'GAMES.MISSES' | translate }}</span>
              </div>
              <div class="stat-box">
                <span class="stat-value">{{ formatTime(elapsed()) }}</span>
                <span class="stat-label">{{ 'GAMES.TIME' | translate }}</span>
              </div>
            </div>

            <div class="stars-row">
              @for (s of [1,2,3]; track s) {
                <span class="star" [class.earned]="starCount() >= s">★</span>
              }
            </div>

            <div class="result-btns">
              <button class="btn btn-primary" (click)="restartGame()">{{ 'GAMES.PLAY_AGAIN' | translate }}</button>
              <a routerLink="/games" class="btn btn-secondary">{{ 'COMMON.BACK' | translate }}</a>
            </div>
          </div>
        } @else {
          <!-- Game Header -->
          <div class="game-header">
            <div class="header-left">
              <h1>🃏 {{ title() || ('GAMES.FLIPCARD' | translate) }}</h1>
              @if (instructions()) {
                <p class="game-instructions">{{ instructions() }}</p>
              }
            </div>
            <div class="game-info">
              <span class="game-score">{{ 'GAMES.SCORE' | translate }}: {{ score() }}</span>
              <span class="game-flips">🔄 {{ totalFlips() }} {{ 'GAMES.FLIPS' | translate }}</span>
              <span class="game-time">⏱️ {{ formatTime(elapsed()) }}</span>
              <span class="match-pill">{{ matchedCount() }} / {{ totalPairs() }}</span>
            </div>
          </div>

          <!-- Card Grid -->
          <div class="card-grid" [class.cols-4]="cards().length <= 16" [class.cols-5]="cards().length > 16 && cards().length <= 20" [class.cols-6]="cards().length > 20">
            @for (card of cards(); track card.id; let i = $index) {
              <div class="flip-card"
                   [class.flipped]="card.flipped || card.matched"
                   [class.matched]="card.matched"
                   [class.mismatch]="card.mismatch"
                   [style.animation-delay]="i * 0.03 + 's'"
                   (click)="flipCard(card)">
                <div class="flip-card-inner">
                  <div class="flip-card-front">
                    <span class="card-back-icon">❓</span>
                    <div class="card-shimmer"></div>
                  </div>
                  <div class="flip-card-back">
                    @if (card.imageUrl) {
                      <img [src]="card.imageUrl" [alt]="card.text" class="card-img">
                    }
                    <span class="card-text" [class.small]="card.text.length > 15">{{ card.text }}</span>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .game-page { padding: 2rem 0 5rem; }

    .game-header {
      display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 2rem; flex-wrap: wrap; gap: 1rem;
      h1 { font-size: 1.5rem; font-weight: 800; }
      .game-instructions { color: #6b7280; font-size: 0.9rem; margin-top: 0.25rem; }
    }
    .game-info { display: flex; gap: 1rem; align-items: center; flex-wrap: wrap; }
    .game-score { font-weight: 800; color: #10b981; font-size: 1.1rem; }
    .game-flips { font-weight: 600; color: #a855f7; }
    .game-time { font-weight: 600; color: #6b7280; }
    .match-pill {
      background: linear-gradient(135deg, #10b981, #34d399); color: white;
      padding: 0.3rem 0.8rem; border-radius: 20px; font-weight: 700; font-size: 0.85rem;
    }

    /* Card Grid */
    .card-grid {
      display: grid; gap: 0.8rem; max-width: 900px; margin: 0 auto;
      &.cols-4 { grid-template-columns: repeat(4, 1fr); }
      &.cols-5 { grid-template-columns: repeat(5, 1fr); }
      &.cols-6 { grid-template-columns: repeat(6, 1fr); }
      @media(max-width: 768px) {
        &.cols-5, &.cols-6 { grid-template-columns: repeat(4, 1fr); }
      }
      @media(max-width: 480px) {
        &.cols-4, &.cols-5, &.cols-6 { grid-template-columns: repeat(3, 1fr); }
        gap: 0.5rem;
      }
    }

    .flip-card {
      aspect-ratio: 3/4; perspective: 800px; cursor: pointer;
      animation: fadeInUp 0.4s ease forwards;
      &.matched { pointer-events: none; }
    }

    .flip-card-inner {
      width: 100%; height: 100%; position: relative;
      transition: transform 0.5s cubic-bezier(0.4, 0, 0.2, 1);
      transform-style: preserve-3d;
    }
    .flip-card.flipped .flip-card-inner,
    .flip-card.matched .flip-card-inner {
      transform: rotateY(180deg);
    }

    .flip-card-front, .flip-card-back {
      position: absolute; inset: 0; border-radius: 14px;
      backface-visibility: hidden; display: flex; flex-direction: column;
      align-items: center; justify-content: center; padding: 0.5rem;
    }

    .flip-card-front {
      background: linear-gradient(145deg, #312e81, #4338ca);
      border: 3px solid #4f46e5;
      overflow: hidden;
      .card-back-icon { font-size: 2.5rem; opacity: 0.6; }
      .card-shimmer {
        position: absolute; inset: 0;
        background: linear-gradient(135deg, transparent 40%, rgba(255,255,255,0.08) 50%, transparent 60%);
        animation: shimmer 3s infinite;
      }
    }

    .flip-card-back {
      background: white; border: 3px solid #e5e7eb;
      transform: rotateY(180deg);
      box-shadow: 0 2px 10px rgba(0,0,0,0.06);
      .card-img { width: 60%; max-height: 50%; object-fit: contain; margin-bottom: 0.5rem; border-radius: 8px; }
      .card-text {
        font-weight: 700; color: #1f2937; text-align: center; font-size: 0.95rem; line-height: 1.3;
        &.small { font-size: 0.8rem; }
      }
    }

    .flip-card.matched {
      .flip-card-back { border-color: #10b981; background: #ecfdf5; }
      animation: matchPulse 0.5s ease;
    }
    .flip-card.mismatch {
      .flip-card-back { border-color: #ef4444; background: #fef2f2; }
      animation: shake 0.4s ease;
    }

    /* Result */
    .result-card {
      max-width: 440px; margin: 3rem auto; text-align: center;
      background: white; border-radius: 24px; padding: 3rem;
      box-shadow: 0 10px 40px rgba(0,0,0,0.08);
      .result-emoji { font-size: 5rem; display: block; margin-bottom: 1rem; animation: float 2s ease-in-out infinite; }
      h2 { font-size: 1.5rem; font-weight: 800; color: #1f2937; margin-bottom: 1rem; }
      .final-score { font-size: 3rem; font-weight: 900; color: #10b981; }
      p { color: #6b7280; margin-bottom: 1.5rem; }
      .result-btns { display: flex; gap: 1rem; justify-content: center; margin-top: 1.5rem; }
    }
    .stats-row {
      display: flex; gap: 1rem; justify-content: center; margin-bottom: 1rem;
      .stat-box {
        background: #f9fafb; border-radius: 14px; padding: 0.75rem 1.25rem; text-align: center;
        .stat-value { display: block; font-size: 1.5rem; font-weight: 800; color: #1f2937; }
        .stat-label { font-size: 0.75rem; color: #9ca3af; font-weight: 600; text-transform: uppercase; }
      }
    }
    .stars-row {
      display: flex; justify-content: center; gap: 0.5rem; margin-bottom: 0.5rem;
      .star { font-size: 2.5rem; color: #e5e7eb; transition: all 0.3s;
        &.earned { color: #f59e0b; filter: drop-shadow(0 2px 6px rgba(245,158,11,0.4)); animation: starBounce 0.5s ease; }
      }
    }

    @keyframes shimmer { 0%{transform:translateX(-100%)} 100%{transform:translateX(100%)} }
    @keyframes matchPulse { 0%{transform:scale(1)} 50%{transform:scale(1.08)} 100%{transform:scale(1)} }
    @keyframes shake { 0%,100%{transform:translateX(0)} 25%{transform:translateX(-6px)} 75%{transform:translateX(6px)} }
    @keyframes fadeInUp { from{opacity:0;transform:translateY(20px)} to{opacity:1;transform:translateY(0)} }
    @keyframes starBounce { 0%{transform:scale(0)} 50%{transform:scale(1.3)} 100%{transform:scale(1)} }
  `]
})
export class FlipcardComponent implements OnInit, OnDestroy {
  loading = signal(true);
  gameOver = signal(false);
  score = signal(0);
  elapsed = signal(0);
  totalFlips = signal(0);
  wrongFlips = signal(0);
  matchedCount = signal(0);
  totalPairs = signal(0);
  title = signal('');
  instructions = signal('');
  cards = signal<Card[]>([]);

  private timer: any;
  private sessionId = 0;
  private flippedCards: Card[] = [];
  private lockBoard = false;
  private pointsPerMatch = 100;
  private movePenalty = 10;

  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router, private translate: TranslateService) {}

  scorePercent(): number {
    return this.totalPairs() > 0 ? Math.round((this.matchedCount() / this.totalPairs()) * 100) : 0;
  }

  starCount(): number {
    const pct = this.scorePercent();
    const efficiency = this.totalFlips() > 0 ? (this.matchedCount() * 2) / this.totalFlips() : 0;
    if (pct === 100 && efficiency > 0.6) return 3;
    if (pct === 100) return 2;
    if (pct >= 50) return 1;
    return 0;
  }

  ngOnInit() {
    const gameId = +this.route.snapshot.params['id'];
    this.api.startFlipCardGame(gameId).subscribe({
      next: res => {
        this.loading.set(false);
        if (res.success && res.data) {
          this.sessionId = res.data.sessionId || res.data.id;
          this.title.set(res.data.gameTitle || '');
          this.instructions.set(res.data.instructions || '');
          this.pointsPerMatch = res.data.pointsPerMatch || 100;
          this.totalPairs.set(res.data.numberOfPairs || (res.data.pairs?.length || 0));

          const pairs = res.data.pairs || [];
          this.buildCards(pairs);
          this.startTimer();
        } else {
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

  private buildCards(pairs: any[]) {
    const cards: Card[] = [];
    let id = 0;

    pairs.forEach((pair: any, idx: number) => {
      // Card 1
      cards.push({
        id: id++,
        pairId: idx,
        text: pair.card1Text || pair.questionText || `Card ${idx * 2 + 1}`,
        imageUrl: pair.card1ImageUrl,
        audioUrl: pair.card1AudioUrl,
        flipped: false,
        matched: false,
        mismatch: false
      });
      // Card 2
      cards.push({
        id: id++,
        pairId: idx,
        text: pair.card2Text || pair.answerText || `Card ${idx * 2 + 2}`,
        imageUrl: pair.card2ImageUrl,
        audioUrl: pair.card2AudioUrl,
        flipped: false,
        matched: false,
        mismatch: false
      });
    });

    // Fisher-Yates shuffle
    for (let i = cards.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [cards[i], cards[j]] = [cards[j], cards[i]];
    }

    this.cards.set(cards);
    this.totalPairs.set(pairs.length);
  }

  private loadDemoData() {
    this.title.set(this.translate.instant('GAMES.ANIMALS_GAME'));
    this.instructions.set(this.translate.instant('GAMES.FIND_PAIRS'));
    const demoPairs = [
      { card1Text: '🐕 Dog', card2Text: 'Puppy' },
      { card1Text: '🐈 Cat', card2Text: 'Kitten' },
      { card1Text: '🐦 Bird', card2Text: 'Chick' },
      { card1Text: '🐟 Fish', card2Text: 'Fry' },
      { card1Text: '🐻 Bear', card2Text: 'Cub' },
      { card1Text: '🦁 Lion', card2Text: 'Lioness' },
    ];
    this.buildCards(demoPairs);
  }

  flipCard(card: Card) {
    if (this.lockBoard || card.flipped || card.matched) return;

    card.flipped = true;
    card.mismatch = false;
    this.totalFlips.set(this.totalFlips() + 1);
    this.flippedCards.push(card);
    this.cards.set([...this.cards()]);

    if (this.flippedCards.length === 2) {
      this.lockBoard = true;
      const [first, second] = this.flippedCards;

      if (first.pairId === second.pairId) {
        // Match!
        setTimeout(() => {
          first.matched = true;
          second.matched = true;
          this.matchedCount.set(this.matchedCount() + 1);
          this.score.set(this.score() + this.pointsPerMatch);
          this.cards.set([...this.cards()]);
          this.flippedCards = [];
          this.lockBoard = false;

          // Check if all matched
          if (this.matchedCount() === this.totalPairs()) {
            this.endGame();
          }
        }, 600);
      } else {
        // Mismatch
        this.wrongFlips.set(this.wrongFlips() + 1);
        setTimeout(() => {
          first.mismatch = true;
          second.mismatch = true;
          this.cards.set([...this.cards()]);
        }, 500);

        setTimeout(() => {
          first.flipped = false;
          second.flipped = false;
          first.mismatch = false;
          second.mismatch = false;
          this.cards.set([...this.cards()]);
          this.flippedCards = [];
          this.lockBoard = false;
        }, 1200);
      }
    }
  }

  private endGame() {
    clearInterval(this.timer);

    // Submit to backend
    this.api.submitFlipCardGame({
      sessionId: this.sessionId,
      matchesFound: this.matchedCount(),
      totalFlips: this.totalFlips(),
      wrongFlips: this.wrongFlips(),
      hintsUsed: 0,
      timeSpentSeconds: this.elapsed()
    }).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.score.set(res.data.totalScore);
        }
        this.gameOver.set(true);
      },
      error: () => {
        this.gameOver.set(true);
      }
    });
  }

  restartGame() { location.reload(); }

  private startTimer() {
    this.timer = setInterval(() => this.elapsed.set(this.elapsed() + 1), 1000);
  }

  formatTime(s: number): string {
    return `${Math.floor(s / 60)}:${(s % 60).toString().padStart(2, '0')}`;
  }
}
