import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
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
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Attach a user-friendly Arabic message to the error
      const friendlyMessage = errorMessages[error.status] || errorMessages[500];
      const enhancedError = {
        ...error,
        friendlyMessage,
        originalMessage: error.message
      };
      return throwError(() => enhancedError);
    })
  );
};
