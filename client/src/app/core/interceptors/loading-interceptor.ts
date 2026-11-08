import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { BusyService } from '../services/busy.service';
import { environment } from '../../../environments/environment.development';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busy = inject(BusyService)

  busy.busy();
  return next(req).pipe(
    (environment.production ? identity : delay(500)),
    finalize(() => busy.idle())
  );
};
