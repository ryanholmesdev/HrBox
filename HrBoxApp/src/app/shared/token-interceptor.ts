import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
	constructor() {}

	intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		let httpOptions = {
			withCredentials: true
		};

		let token = '';

		request = request.clone({
			setHeaders: {
				Authorization: `Bearer ${token}`
			}
		});

		const authReq = request.clone(httpOptions);

		return next.handle(authReq);
	}
}
