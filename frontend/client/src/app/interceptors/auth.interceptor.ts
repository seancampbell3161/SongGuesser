import { HttpInterceptorFn } from '@angular/common/http';
import { tap } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');
  console.log(token);

  const reqWithHeader = req.clone({
    headers: req.headers.append('Authorization', token ? `Bearer ${token}` : ''),
  });

  return next(reqWithHeader);
};
