import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

/** Maps HTTP status codes to child-friendly Arabic error messages */
const errorMessages: Record<number, string> = {
  0:   'لا يوجد اتصال بالإنترنت. تحقق من الشبكة وحاول مجدداً 📡',
  400: 'حدث خطأ في البيانات المرسلة. حاول مجدداً ⚠️',
  401: 'انتهت جلستك. سجّل دخولك مجدداً 🔑',
  403: 'عذراً، هذه الصفحة غير متاحة لك 🚫',
  404: 'المحتوى الذي تبحث عنه غير موجود 🔍',
  429: 'حاولت كثيراً! انتظر قليلاً ثم حاول مجدداً ⏳',
  500: 'حدث خطأ في الخادم. نعمل على إصلاحه! 🛠️',
  502: 'الخادم غير متاح حالياً. حاول بعد قليل 🔄',
  503: 'الخدمة غير متاحة مؤقتاً. حاول لاحقاً ⏳',
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const friendlyMessage = errorMessages[error.status] || errorMessages[500];

      // Navigate to 500 page on server errors (but NOT during test-taking)
      if (error.status >= 500 && !router.url.includes('/take')) {
        router.navigate(['/error/500']);
      }

      const enhancedError = {
        ...error,
        friendlyMessage,
        originalMessage: error.message
      };
      return throwError(() => enhancedError);
    })
  );
};
