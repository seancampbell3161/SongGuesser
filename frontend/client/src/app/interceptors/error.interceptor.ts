import { HttpErrorResponse, HttpEventType, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, EMPTY, of, tap, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageSvc = inject(MessageService);

  return next(req).pipe(catchError((err) => {
    if (err instanceof HttpErrorResponse) {
      if (err.status >= 400) {
        let detail = 'An unexpected error occurred. Please try again later';

        if (err.message) {
          detail = err.message;
        }

        if (err.error && typeof err.error === 'string') {
          console.error(err.error);
        }

        messageSvc.add({ severity: 'error', summary: 'Error', detail: detail });
      }
    }
    return throwError(() => err);
  }))
};