import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, EMPTY, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authSvc = inject(AuthService);
  const token = localStorage.getItem('token');

  const reqWithHeader = req.clone({
    headers: req.headers.append('Authorization', token ? `Bearer ${token}` : '')
  });

  return next(reqWithHeader).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 && !req.url.endsWith('refresh')) {
        return authSvc.refreshToken().pipe(
          switchMap((token) => {
            const retryRequest = req.clone({
              setHeaders: {
                Authorization: `Bearer ${token}` 
              },
            })

            return next(retryRequest);
          })
        );
      }

      return EMPTY;
    }) 
  );
};