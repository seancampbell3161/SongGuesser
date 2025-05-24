import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');

  const reqWithHeader = req.clone({
    headers: req.headers.append('Authorization', token ? `Bearer ${token}` : ''),
  });

  return next(reqWithHeader);
};