import { Injectable } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { TranslateService } from '@ngx-translate/core';

interface SeoConfig {
  titleKey?: string;
  titleDefault?: string;
  descriptionKey?: string;
  descriptionDefault?: string;
  keywords?: string;
  ogImage?: string;
}

@Injectable({ providedIn: 'root' })
export class SeoService {
  private readonly siteNameAr = 'الإنجليزية مع ديما';
  private readonly siteNameEn = 'English with Dima';
  private readonly baseUrl = 'https://englishwithdima.com';
  private readonly defaultImage = '/assets/images/og-default.png';

  constructor(
    private title: Title,
    private meta: Meta,
    private translate: TranslateService
  ) {}

  private get siteName(): string {
    return this.translate.currentLang === 'ar' ? this.siteNameAr : this.siteNameEn;
  }

  /**
   * Set page SEO meta tags. Call from each page component's ngOnInit().
   */
  setPage(config: SeoConfig): void {
    // Title
    const pageTitle = config.titleKey
      ? this.translate.instant(config.titleKey)
      : (config.titleDefault || this.siteName);
    this.title.setTitle(`${pageTitle} | ${this.siteName}`);

    // Description
    const desc = config.descriptionKey
      ? this.translate.instant(config.descriptionKey)
      : (config.descriptionDefault || 'تعلم الإنجليزية من خلال اختبارات تفاعلية وألعاب تعليمية — الإنجليزية مع ديما');
    this.updateTag('description', desc);

    // Keywords
    if (config.keywords) {
      this.updateTag('keywords', config.keywords);
    }

    // Open Graph
    this.updateProperty('og:title', pageTitle);
    this.updateProperty('og:description', desc);
    this.updateProperty('og:image', config.ogImage || this.defaultImage);
    this.updateProperty('og:type', 'website');
    this.updateProperty('og:site_name', this.siteName);

    // Twitter Card
    this.updateTag('twitter:card', 'summary_large_image');
    this.updateTag('twitter:title', pageTitle);
    this.updateTag('twitter:description', desc);
    this.updateTag('twitter:image', config.ogImage || this.defaultImage);
  }

  /**
   * Set page for a specific test.
   */
  setTestPage(testTitle: string, gradeLevel: string): void {
    this.setPage({
      titleDefault: testTitle,
      descriptionDefault: `${testTitle} — اختبار إنجليزي ${gradeLevel} على منصة الإنجليزية مع ديما`,
      keywords: `اختبار إنجليزي, ${gradeLevel}, ${testTitle}, تعلم الإنجليزية, الإنجليزية مع ديما`
    });
  }

  /**
   * Set page for a specific game.
   */
  setGamePage(gameName: string, gameType: string): void {
    this.setPage({
      titleDefault: gameName,
      descriptionDefault: `العب ${gameName} — لعبة ${gameType} لتعلم الإنجليزية على منصة الإنجليزية مع ديما`,
      keywords: `لعبة إنجليزي, ${gameType}, ${gameName}, تعلم الإنجليزية, ألعاب تعليمية`
    });
  }

  /**
   * Reset to default site title.
   */
  resetToDefault(): void {
    this.title.setTitle(`${this.siteNameAr} — تعلم الإنجليزية بطريقة ممتعة`);
    this.updateTag('description',
      'تعلم الإنجليزية مع ديما — اختبارات تفاعلية وألعاب تعليمية ومحتوى مخصص لطلاب مصر. منصة تعليمية متكاملة للمرحلة الابتدائية والإعدادية');
  }

  private updateTag(name: string, content: string): void {
    const existing = this.meta.getTag(`name="${name}"`);
    if (existing) {
      this.meta.updateTag({ name, content });
    } else {
      this.meta.addTag({ name, content });
    }
  }

  private updateProperty(property: string, content: string): void {
    const existing = this.meta.getTag(`property="${property}"`);
    if (existing) {
      this.meta.updateTag({ property, content });
    } else {
      this.meta.addTag({ property, content });
    }
  }
}
