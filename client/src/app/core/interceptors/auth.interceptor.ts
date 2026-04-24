import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  // Skip auth for public endpoints
  const publicPaths = ['/auth/login', '/auth/register', '/auth/guest', '/auth/student-login', '/contact/'];
  const isPublic = publicPaths.some(p => req.url.includes(p));

  if (token && !isPublic) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('/auth/refresh')) {
        // Try to refresh token
        return auth.refreshToken().pipe(
          switchMap(res => {
            if (res.success) {
              const newReq = req.clone({
                setHeaders: { Authorization: `Bearer ${res.data.accessToken}` }
              });
              return next(newReq);
            }
            auth.logout();
            return throwError(() => error);
          }),
          catchError(() => {
            auth.logout();
            return throwError(() => error);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
