import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../../core/services/api.service';
import { Grade } from '../../../core/models';

@Component({
  selector: 'app-admin-games',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  template: `
    <section class="admin-page">
      <div class="container">
        <div class="admin-header">
          <h1>{{ 'ADMIN.GAMES_MGMT' | translate }}</h1>
        </div>

        <!-- Game Type Tabs -->
        <div class="game-tabs">
          @for (tab of gameTabs; track tab.key) {
            <button class="tab" [class.active]="activeTab() === tab.key" (click)="activeTab.set(tab.key)">
              {{ tab.icon }} {{ 'ADMIN.' + tab.labelKey | translate }}
            </button>
          }
        </div>

        <!-- ==================== MATCHING GAMES TAB ==================== -->
        @if (activeTab() === 'matching') {
          <div class="game-section animate-fade-in">
            <div class="section-header">
              <h2>{{ 'ADMIN.MATCHING_GAMES' | translate }}</h2>
              <button class="btn btn-primary btn-sm" (click)="showMatchForm.set(!showMatchForm())">{{ showMatchForm() ? '✕' : ('ADMIN.NEW_GAME' | translate) }}</button>
            </div>
            @if (showMatchForm()) {
              <div class="form-card animate-scale-in">
                <form (ngSubmit)="createMatching()">
                  <div class="form-row-3">
                    <div class="form-group"><label>{{ 'ADMIN.GAME_TITLE' | translate }} *</label><input type="text" [(ngModel)]="matchForm.gameTitle" name="title" required></div>
                    <div class="form-group"><label>{{ 'ADMIN.GRADE' | translate }} *</label><select [(ngModel)]="matchForm.gradeId" name="grade" required>@for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }</select></div>
                    <div class="form-group"><label>{{ 'ADMIN.DIFFICULTY' | translate }}</label><select [(ngModel)]="matchForm.difficultyLevel" name="diff"><option value="Easy">{{ 'ADMIN.EASY' | translate }}</option><option value="Medium">{{ 'ADMIN.MEDIUM' | translate }}</option><option value="Hard">{{ 'ADMIN.HARD' | translate }}</option></select></div>
                  </div>
                  <div class="form-group"><label>{{ 'ADMIN.GAME_INSTRUCTIONS' | translate }}</label><textarea [(ngModel)]="matchForm.instructions" name="instructions" rows="2"></textarea></div>

                  <h3 class="pairs-title">{{ 'ADMIN.MATCHING_PAIRS' | translate }}</h3>
                  @for (pair of matchForm.pairs; track pair.pairOrder; let i = $index) {
                    <div class="pair-row">
                      <input type="text" [(ngModel)]="pair.questionText" [name]="'mq_'+i" [placeholder]="'ADMIN.QUESTION_WORD' | translate">
                      <span class="pair-arrow">↔</span>
                      <input type="text" [(ngModel)]="pair.answerText" [name]="'ma_'+i" [placeholder]="'ADMIN.ANSWER_MATCH' | translate">
                      <button type="button" class="btn-remove" (click)="matchForm.pairs.splice(i,1)">✕</button>
                    </div>
                  }
                  <button type="button" class="btn btn-sm btn-secondary" (click)="addMatchPair()">{{ 'ADMIN.ADD_PAIR' | translate }}</button>
                  <div class="form-actions">
                    <button type="submit" class="btn btn-primary" [disabled]="saving()">{{ 'ADMIN.CREATE_GAME' | translate }}</button>
                  </div>
                </form>
              </div>
            }
            <div class="games-list">
              @for (g of matchingGames(); track g.id) {
                <div class="game-list-card">
                  <div class="glc-icon" style="background:#6366f115;color:#6366f1">🔗</div>
                  <div class="glc-info"><h4>{{ g.gameTitle }}</h4><span>{{ g.numberOfPairs }} {{ 'ADMIN.PAIRS' | translate }} · {{ 'ENUMS.DIFFICULTY.' + g.difficultyLevel | translate }}</span></div>
                  <button class="btn btn-sm btn-danger" (click)="deleteGame('matching', g.id, g.gameTitle)">{{ 'ADMIN.DELETE' | translate }}</button>
                </div>
              } @empty { <p class="empty-text">{{ 'ADMIN.NO_MATCHING' | translate }}</p> }
            </div>
          </div>
        }

        <!-- ==================== WHEEL GAMES TAB ==================== -->
        @if (activeTab() === 'wheel') {
          <div class="game-section animate-fade-in">
            <div class="section-header"><h2>{{ 'ADMIN.WHEEL_GAMES' | translate }}</h2></div>
            <div class="info-panel"><p>{{ 'ADMIN.WHEEL_INFO' | translate }}</p></div>
          </div>
        }

        <!-- ==================== DRAG & DROP GAMES TAB ==================== -->
        @if (activeTab() === 'dragdrop') {
          <div class="game-section animate-fade-in">
            <div class="section-header">
              <h2>{{ 'ADMIN.DRAGDROP_GAMES' | translate }}</h2>
              <button class="btn btn-primary btn-sm" (click)="showDragForm.set(!showDragForm())">{{ showDragForm() ? '✕' : ('ADMIN.NEW_GAME' | translate) }}</button>
            </div>
            @if (showDragForm()) {
              <div class="form-card animate-scale-in">
                <form (ngSubmit)="createDragDrop()">
                  <div class="form-row-3">
                    <div class="form-group"><label>{{ 'ADMIN.GAME_TITLE' | translate }} *</label><input type="text" [(ngModel)]="dragForm.gameTitle" name="dtitle" required></div>
                    <div class="form-group"><label>{{ 'ADMIN.GRADE' | translate }} *</label><select [(ngModel)]="dragForm.gradeId" name="dgrade" required>@for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }</select></div>
                    <div class="form-group"><label>{{ 'ADMIN.DIFFICULTY' | translate }}</label><select [(ngModel)]="dragForm.difficultyLevel" name="ddiff"><option value="Easy">{{ 'ADMIN.EASY' | translate }}</option><option value="Medium">{{ 'ADMIN.MEDIUM' | translate }}</option><option value="Hard">{{ 'ADMIN.HARD' | translate }}</option></select></div>
                  </div>
                  <div class="form-group"><label>{{ 'ADMIN.GAME_INSTRUCTIONS' | translate }}</label><textarea [(ngModel)]="dragForm.instructions" name="dinstructions" rows="2"></textarea></div>

                  <!-- Zones -->
                  <h3 class="pairs-title">{{ 'ADMIN.DROP_ZONES' | translate }}</h3>
                  @for (zone of dragForm.zones; track zone.zoneOrder; let i = $index) {
                    <div class="pair-row">
                      <input type="text" [(ngModel)]="zone.zoneLabel" [name]="'zl_'+i" [placeholder]="'ADMIN.ZONE_LABEL' | translate">
                      <input type="text" [(ngModel)]="zone.zoneColor" [name]="'zc_'+i" placeholder="#6366f1" class="color-input">
                      <button type="button" class="btn-remove" (click)="dragForm.zones.splice(i,1)">✕</button>
                    </div>
                  }
                  <button type="button" class="btn btn-sm btn-secondary" (click)="addDragZone()">{{ 'ADMIN.ADD_ZONE' | translate }}</button>

                  <!-- Items -->
                  <h3 class="pairs-title">{{ 'ADMIN.DRAG_ITEMS' | translate }}</h3>
                  @for (item of dragForm.items; track item.itemOrder; let i = $index) {
                    <div class="pair-row">
                      <input type="text" [(ngModel)]="item.itemText" [name]="'di_'+i" [placeholder]="'ADMIN.ITEM_TEXT' | translate">
                      <span class="pair-arrow">→</span>
                      <select [(ngModel)]="item.correctZoneIndex" [name]="'dz_'+i">
                        @for (zone of dragForm.zones; track zone.zoneOrder; let zi = $index) {
                          <option [ngValue]="zi">{{ zone.zoneLabel || ('ADMIN.ZONE' | translate) + ' ' + (zi+1) }}</option>
                        }
                      </select>
                      <button type="button" class="btn-remove" (click)="dragForm.items.splice(i,1)">✕</button>
                    </div>
                  }
                  <button type="button" class="btn btn-sm btn-secondary" (click)="addDragItem()">{{ 'ADMIN.ADD_ITEM' | translate }}</button>

                  <div class="form-actions">
                    <button type="submit" class="btn btn-primary" [disabled]="saving()">{{ 'ADMIN.CREATE_GAME' | translate }}</button>
                  </div>
                </form>
              </div>
            }
            <div class="games-list">
              @for (g of dragDropGames(); track g.id) {
                <div class="game-list-card">
                  <div class="glc-icon" style="background:#06b6d415;color:#06b6d4">✋</div>
                  <div class="glc-info"><h4>{{ g.gameTitle }}</h4><span>{{ g.zones?.length || 0 }} {{ 'ADMIN.ZONES' | translate }} · {{ g.items?.length || 0 }} {{ 'ADMIN.ITEMS' | translate }} · {{ 'ENUMS.DIFFICULTY.' + g.difficultyLevel | translate }}</span></div>
                  <button class="btn btn-sm btn-danger" (click)="deleteGame('dragdrop', g.id, g.gameTitle)">{{ 'ADMIN.DELETE' | translate }}</button>
                </div>
              } @empty { <p class="empty-text">{{ 'ADMIN.NO_DRAGDROP' | translate }}</p> }
            </div>
          </div>
        }

        <!-- ==================== FLIP CARD GAMES TAB ==================== -->
        @if (activeTab() === 'flipcard') {
          <div class="game-section animate-fade-in">
            <div class="section-header">
              <h2>{{ 'ADMIN.FLIPCARD_GAMES' | translate }}</h2>
              <button class="btn btn-primary btn-sm" (click)="showFlipForm.set(!showFlipForm())">{{ showFlipForm() ? '✕' : ('ADMIN.NEW_GAME' | translate) }}</button>
            </div>
            @if (showFlipForm()) {
              <div class="form-card animate-scale-in">
                <form (ngSubmit)="createFlipCard()">
                  <div class="form-row-3">
                    <div class="form-group"><label>{{ 'ADMIN.GAME_TITLE' | translate }} *</label><input type="text" [(ngModel)]="flipForm.gameTitle" name="ftitle" required></div>
                    <div class="form-group"><label>{{ 'ADMIN.GRADE' | translate }} *</label><select [(ngModel)]="flipForm.gradeId" name="fgrade" required>@for (g of grades(); track g.id) { <option [ngValue]="g.id">{{ g.nameAr }}</option> }</select></div>
                    <div class="form-group"><label>{{ 'ADMIN.DIFFICULTY' | translate }}</label><select [(ngModel)]="flipForm.difficultyLevel" name="fdiff"><option value="Easy">{{ 'ADMIN.EASY' | translate }}</option><option value="Medium">{{ 'ADMIN.MEDIUM' | translate }}</option><option value="Hard">{{ 'ADMIN.HARD' | translate }}</option></select></div>
                  </div>
                  <div class="form-group"><label>{{ 'ADMIN.GAME_INSTRUCTIONS' | translate }}</label><textarea [(ngModel)]="flipForm.instructions" name="finstructions" rows="2"></textarea></div>

                  <h3 class="pairs-title">{{ 'ADMIN.CARD_PAIRS' | translate }}</h3>
                  @for (pair of flipForm.pairs; track pair.pairOrder; let i = $index) {
                    <div class="pair-row">
                      <input type="text" [(ngModel)]="pair.card1Text" [name]="'fc1_'+i" [placeholder]="'ADMIN.CARD_1' | translate">
                      <span class="pair-arrow">↔</span>
                      <input type="text" [(ngModel)]="pair.card2Text" [name]="'fc2_'+i" [placeholder]="'ADMIN.CARD_2' | translate">
                      <button type="button" class="btn-remove" (click)="flipForm.pairs.splice(i,1)">✕</button>
                    </div>
                  }
                  <button type="button" class="btn btn-sm btn-secondary" (click)="addFlipPair()">{{ 'ADMIN.ADD_PAIR' | translate }}</button>
                  <div class="form-actions">
                    <button type="submit" class="btn btn-primary" [disabled]="saving()">{{ 'ADMIN.CREATE_GAME' | translate }}</button>
                  </div>
                </form>
              </div>
            }
            <div class="games-list">
              @for (g of flipCardGames(); track g.id) {
                <div class="game-list-card">
                  <div class="glc-icon" style="background:#10b98115;color:#10b981">🃏</div>
                  <div class="glc-info"><h4>{{ g.gameTitle }}</h4><span>{{ g.numberOfPairs }} {{ 'ADMIN.PAIRS' | translate }} · {{ 'ENUMS.DIFFICULTY.' + g.difficultyLevel | translate }}</span></div>
                  <button class="btn btn-sm btn-danger" (click)="deleteGame('flipcard', g.id, g.gameTitle)">{{ 'ADMIN.DELETE' | translate }}</button>
                </div>
              } @empty { <p class="empty-text">{{ 'ADMIN.NO_FLIPCARD' | translate }}</p> }
            </div>
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .admin-page { padding:2rem 0 5rem; }
    .admin-header { margin-bottom:1.5rem; h1 { font-size:1.6rem; font-weight:800; } }
    .game-tabs { display:flex; gap:.5rem; margin-bottom:2rem; flex-wrap:wrap; }
    .tab { padding:.6rem 1.25rem; border-radius:12px; border:2px solid #e5e7eb; background:white; font-weight:600; font-size:.9rem; cursor:pointer; transition:all .2s;
      &:hover { border-color:#6366f1; }
      &.active { background:linear-gradient(135deg,#6366f1,#a855f7); color:white; border-color:transparent; }
    }
    .game-section { margin-bottom:2rem; }
    .section-header { display:flex; justify-content:space-between; align-items:center; margin-bottom:1rem; h2 { font-size:1.3rem; font-weight:800; } }
    .form-card { background:white; border-radius:16px; padding:1.5rem; box-shadow:0 4px 20px rgba(0,0,0,.06); margin-bottom:1.5rem; }
    .form-row-3 { display:grid; grid-template-columns:repeat(3,1fr); gap:1rem; @media(max-width:768px){ grid-template-columns:1fr; } }
    .pairs-title { font-size:1rem; font-weight:700; margin:1.25rem 0 .75rem; }
    .pair-row { display:flex; align-items:center; gap:.5rem; margin-bottom:.5rem;
      input, select { flex:1; padding:.5rem .75rem; border:2px solid #e5e7eb; border-radius:8px; font-size:.9rem; }
      .color-input { max-width:100px; }
      .pair-arrow { font-size:1.2rem; color:#6366f1; font-weight:700; flex-shrink:0; }
      .btn-remove { background:none; border:none; cursor:pointer; color:#ef4444; font-size:1rem; flex-shrink:0; }
    }
    .form-actions { display:flex; justify-content:flex-end; margin-top:1rem; }
    .games-list { display:flex; flex-direction:column; gap:.75rem; }
    .game-list-card { display:flex; align-items:center; gap:1rem; background:white; border-radius:14px; padding:1rem 1.25rem; box-shadow:0 2px 10px rgba(0,0,0,.04); transition:all .2s;
      &:hover { transform:translateX(-4px); box-shadow:0 4px 15px rgba(0,0,0,.08); }
      .glc-icon { width:44px; height:44px; border-radius:12px; display:flex; align-items:center; justify-content:center; font-size:1.4rem; flex-shrink:0; }
      .glc-info { flex:1; h4 { font-weight:700; font-size:.95rem; color:#1f2937; } span { font-size:.8rem; color:#6b7280; } }
    }
    .info-panel { background:#f5f3ff; border:1px solid #c4b5fd; border-radius:12px; padding:1.25rem; p { color:#4b5563; font-size:.9rem; } }
    .empty-text { color:#9ca3af; text-align:center; padding:2rem; font-size:.95rem; }
    .btn-danger { background:#ef4444 !important; color:white !important; border:none !important;
      &:hover { background:#dc2626 !important; }
    }
  `]
})
export class AdminGamesComponent implements OnInit {
  grades = signal<Grade[]>([]);
  activeTab = signal('matching');
  saving = signal(false);

  // Form visibility toggles
  showMatchForm = signal(false);
  showDragForm = signal(false);
  showFlipForm = signal(false);

  // Game lists
  matchingGames = signal<any[]>([]);
  dragDropGames = signal<any[]>([]);
  flipCardGames = signal<any[]>([]);

  gameTabs = [
    { key: 'matching', icon: '🔗', labelKey: 'MATCHING_TAB' },
    { key: 'wheel', icon: '🎡', labelKey: 'WHEEL_TAB' },
    { key: 'dragdrop', icon: '✋', labelKey: 'DRAGDROP_TAB' },
    { key: 'flipcard', icon: '🃏', labelKey: 'FLIPCARD_TAB' },
  ];

  // ===== Matching Form =====
  matchForm: any = {
    gameTitle: '', gradeId: 1, difficultyLevel: 'Medium', instructions: '',
    skillCategory: 'Vocabulary', matchingMode: 'Both', pointsPerMatch: 100,
    pairs: [
      { questionText: '', answerText: '', pairOrder: 0 },
      { questionText: '', answerText: '', pairOrder: 1 },
      { questionText: '', answerText: '', pairOrder: 2 }
    ]
  };

  // ===== DragDrop Form =====
  dragForm: any = {
    gameTitle: '', gradeId: 1, difficultyLevel: 'Medium', instructions: '',
    skillCategory: 'Vocabulary', pointsPerCorrectItem: 10,
    zones: [
      { zoneLabel: '', zoneColor: '#6366f1', zoneOrder: 0 },
      { zoneLabel: '', zoneColor: '#a855f7', zoneOrder: 1 },
    ],
    items: [
      { itemText: '', correctZoneIndex: 0, itemOrder: 0 },
      { itemText: '', correctZoneIndex: 0, itemOrder: 1 },
      { itemText: '', correctZoneIndex: 1, itemOrder: 2 },
    ]
  };

  // ===== FlipCard Form =====
  flipForm: any = {
    gameTitle: '', gradeId: 1, difficultyLevel: 'Medium', instructions: '',
    skillCategory: 'Vocabulary', gameMode: 'ClassicMemory', numberOfPairs: 6,
    pointsPerMatch: 100,
    pairs: [
      { card1Text: '', card2Text: '', pairOrder: 0 },
      { card1Text: '', card2Text: '', pairOrder: 1 },
      { card1Text: '', card2Text: '', pairOrder: 2 }
    ]
  };

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getGrades().subscribe(r => { if (r.success) this.grades.set(r.data); });
    this.loadGames();
  }

  loadGames() {
    this.api.getMatchingGames().subscribe(r => { if (r.success) this.matchingGames.set(r.data); });
    this.api.getDragDropGames().subscribe(r => { if (r.success) this.dragDropGames.set(r.data); });
    this.api.getFlipCardGames().subscribe(r => { if (r.success) this.flipCardGames.set(r.data); });
  }

  // ===== Matching CRUD =====
  addMatchPair() { this.matchForm.pairs.push({ questionText: '', answerText: '', pairOrder: this.matchForm.pairs.length }); }

  createMatching() {
    this.saving.set(true);
    this.api.createMatchingGame(this.matchForm).subscribe({
      next: () => {
        this.saving.set(false);
        this.showMatchForm.set(false);
        this.resetMatchForm();
        this.loadGames();
      },
      error: () => this.saving.set(false)
    });
  }

  // ===== DragDrop CRUD =====
  addDragZone() { this.dragForm.zones.push({ zoneLabel: '', zoneColor: '#10b981', zoneOrder: this.dragForm.zones.length }); }
  addDragItem() { this.dragForm.items.push({ itemText: '', correctZoneIndex: 0, itemOrder: this.dragForm.items.length }); }

  createDragDrop() {
    this.saving.set(true);
    this.api.createDragDropGame(this.dragForm).subscribe({
      next: () => {
        this.saving.set(false);
        this.showDragForm.set(false);
        this.resetDragForm();
        this.loadGames();
      },
      error: () => this.saving.set(false)
    });
  }

  // ===== FlipCard CRUD =====
  addFlipPair() { this.flipForm.pairs.push({ card1Text: '', card2Text: '', pairOrder: this.flipForm.pairs.length }); }

  createFlipCard() {
    this.saving.set(true);
    this.api.createFlipCardGame(this.flipForm).subscribe({
      next: () => {
        this.saving.set(false);
        this.showFlipForm.set(false);
        this.resetFlipForm();
        this.loadGames();
      },
      error: () => this.saving.set(false)
    });
  }

  // ===== Delete =====
  deleteGame(type: string, id: number, title: string) {
    if (!confirm(`هل تريد حذف "${title}"؟`)) return;

    const deleteCall = type === 'matching' ? this.api.deleteMatchingGame(id)
      : type === 'dragdrop' ? this.api.deleteDragDropGame(id)
      : this.api.deleteFlipCardGame(id);

    deleteCall.subscribe({ next: () => this.loadGames() });
  }

  // ===== Form Resets =====
  private resetMatchForm() {
    this.matchForm = {
      gameTitle: '', gradeId: this.grades()[0]?.id || 1, difficultyLevel: 'Medium', instructions: '',
      skillCategory: 'Vocabulary', matchingMode: 'Both', pointsPerMatch: 100,
      pairs: [{ questionText: '', answerText: '', pairOrder: 0 }, { questionText: '', answerText: '', pairOrder: 1 }, { questionText: '', answerText: '', pairOrder: 2 }]
    };
  }

  private resetDragForm() {
    this.dragForm = {
      gameTitle: '', gradeId: this.grades()[0]?.id || 1, difficultyLevel: 'Medium', instructions: '',
      skillCategory: 'Vocabulary', pointsPerCorrectItem: 10,
      zones: [{ zoneLabel: '', zoneColor: '#6366f1', zoneOrder: 0 }, { zoneLabel: '', zoneColor: '#a855f7', zoneOrder: 1 }],
      items: [{ itemText: '', correctZoneIndex: 0, itemOrder: 0 }, { itemText: '', correctZoneIndex: 0, itemOrder: 1 }, { itemText: '', correctZoneIndex: 1, itemOrder: 2 }]
    };
  }

  private resetFlipForm() {
    this.flipForm = {
      gameTitle: '', gradeId: this.grades()[0]?.id || 1, difficultyLevel: 'Medium', instructions: '',
      skillCategory: 'Vocabulary', gameMode: 'ClassicMemory', numberOfPairs: 6, pointsPerMatch: 100,
      pairs: [{ card1Text: '', card2Text: '', pairOrder: 0 }, { card1Text: '', card2Text: '', pairOrder: 1 }, { card1Text: '', card2Text: '', pairOrder: 2 }]
    };
  }
}
