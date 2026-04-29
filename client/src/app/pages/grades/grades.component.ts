import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../core/services/api.service';
import { SeoService } from '../../core/services/seo.service';
import { Grade } from '../../core/models';

@Component({
  selector: 'app-grades',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <section class="grades-page">
      <div class="container">
        <div class="page-header animate-fade-in-up">
          <h1 class="section-title">{{ 'HOME.GRADES_TITLE' | translate }}</h1>
          <p class="section-subtitle">{{ 'GRADES.SUBTITLE' | translate }}</p>
        </div>

        @if (loading()) {
          <div class="grades-grid">
            @for (i of [1,2,3,4,5,6]; track i) {
              <div class="grade-card skeleton-card">
                <div class="skeleton" style="width:50px;height:50px;border-radius:14px"></div>
                <div style="flex:1">
                  <div class="skeleton skeleton-title" style="width:70%"></div>
                  <div class="skeleton skeleton-text" style="width:40%;margin-top:8px"></div>
                </div>
              </div>
            }
          </div>
        } @else {
          <div class="school-sections">
            <!-- Primary School -->
            <div class="school-section animate-fade-in-up">
              <div class="section-badge primary">
                {{ 'GRADES.PRIMARY' | translate }}
              </div>
              <div class="grades-grid">
                @for (grade of primaryGrades(); track grade.id; let i = $index) {
                  <a [routerLink]="['/tests']" [queryParams]="{gradeId: grade.id}" class="grade-card" [style.animation-delay]="i * 0.08 + 's'">
                    <div class="grade-number">{{ grade.level }}</div>
                    <div class="grade-details">
                      <h3>{{ grade.nameAr }}</h3>
                      <p>{{ 'COMMON.UNITS' | translate }}: {{ grade.unitsCount }}</p>
                    </div>
                    <div class="grade-meta">
                      <span class="badge badge-primary">{{ grade.unitsCount }} {{ 'COMMON.UNITS' | translate }}</span>
                    </div>
                    <div class="grade-arrow"><i class="fas fa-chevron-left"></i></div>
                  </a>
                }
              </div>
            </div>

            <!-- Preparatory School -->
            <div class="school-section animate-fade-in-up" style="animation-delay:0.3s">
              <div class="section-badge prep">
                {{ 'GRADES.PREP' | translate }}
              </div>
              <div class="grades-grid">
                @for (grade of prepGrades(); track grade.id; let i = $index) {
                  <a [routerLink]="['/tests']" [queryParams]="{gradeId: grade.id}" class="grade-card prep-card" [style.animation-delay]="(i + 6) * 0.08 + 's'">
                    <div class="grade-number prep-num">{{ grade.level - 6 }}</div>
                    <div class="grade-details">
                      <h3>{{ grade.nameAr }}</h3>
                      <p>{{ 'COMMON.UNITS' | translate }}: {{ grade.unitsCount }}</p>
                    </div>
                    <div class="grade-meta">
                      <span class="badge badge-primary">{{ grade.unitsCount }} {{ 'COMMON.UNITS' | translate }}</span>
                    </div>
                    <div class="grade-arrow"><i class="fas fa-chevron-left"></i></div>
                  </a>
                }
              </div>
            </div>
          </div>
        }
      </div>
    </section>
  `,
  styles: [`
    .grades-page { padding:3rem 0 5rem; }
    .page-header { margin-bottom:3rem; }
    .skeleton-card { padding:1.25rem; }
    .school-section { margin-bottom:3rem; }
    .section-badge { display:inline-flex; align-items:center; gap:.5rem; padding:.6rem 1.2rem; border-radius:25px; font-weight:700; font-size:1rem; margin-bottom:1.5rem;
      &.primary { background:rgba(99,102,241,.1); color:#6366f1; }
      &.prep { background:rgba(168,85,247,.1); color:#a855f7; }
    }
    .grades-grid { display:grid; grid-template-columns:repeat(3,1fr); gap:1rem; @media(max-width:768px){ grid-template-columns:repeat(2,1fr); } @media(max-width:480px){ grid-template-columns:1fr; } }
    .grade-card { display:flex; align-items:center; gap:1rem; background:white; border:2px solid #e5e7eb; border-radius:16px; padding:1.25rem; text-decoration:none; transition:all .3s; animation:fadeInUp .5s ease forwards; opacity:0;
      &:hover { border-color:#6366f1; background:#f5f3ff; transform:translateY(-3px); box-shadow:0 8px 25px rgba(99,102,241,.12); }
      &.prep-card:hover { border-color:#a855f7; background:#faf5ff; box-shadow:0 8px 25px rgba(168,85,247,.12); }
    }
    .grade-number { width:50px; height:50px; border-radius:14px; background:linear-gradient(135deg,#6366f1,#818cf8); color:white; display:flex; align-items:center; justify-content:center; font-size:1.5rem; font-weight:900; flex-shrink:0;
      &.prep-num { background:linear-gradient(135deg,#a855f7,#c084fc); }
    }
    .grade-details { flex:1; h3 { font-weight:700; font-size:1rem; color:#1f2937; } p { font-size:.8rem; color:#6b7280; margin-top:.15rem; } }
    .grade-meta { @media(max-width:600px){ display:none; } }
    .grade-arrow { color:#9ca3af; transition:transform .3s; }
    .grade-card:hover .grade-arrow { transform:translateX(-4px); color:#6366f1; }
  `]
})
export class GradesComponent implements OnInit {
  grades = signal<Grade[]>([]);
  primaryGrades = signal<Grade[]>([]);
  prepGrades = signal<Grade[]>([]);
  loading = signal(true);

  constructor(private api: ApiService, private seo: SeoService) {}

  ngOnInit() {
    this.seo.setPage({
      titleDefault: 'الصفوف الدراسية',
      descriptionDefault: 'اختر صفك الدراسي للبدء في اختبارات الإنجليزية — ابتدائي وإعدادي',
      keywords: 'صفوف دراسية, ابتدائي, إعدادي, اختبارات إنجليزي, الإنجليزية مع ديما'
    });
    this.api.getGrades().subscribe({
      next: res => {
        if (res.success) {
          this.grades.set(res.data);
          this.primaryGrades.set(res.data.filter(g => g.level <= 6));
          this.prepGrades.set(res.data.filter(g => g.level > 6));
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
