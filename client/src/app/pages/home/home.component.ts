import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService } from '../../core/services/api.service';
import { SeoService } from '../../core/services/seo.service';
import { Grade } from '../../core/models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  grades = signal<Grade[]>([]);

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

  constructor(private api: ApiService, private seo: SeoService) {}

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
}
