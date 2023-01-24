import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';

//We already have the token.

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService:AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //this subscription will complete and it will no longer consume resources in our app.
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user=>{
        if (user) {
          request=request.clone({
            setHeaders:{
              Authorization: ("Bearer" +' '+user.token)
            }
          })
        }
      }
    })
    return next.handle(request);
  }
}
