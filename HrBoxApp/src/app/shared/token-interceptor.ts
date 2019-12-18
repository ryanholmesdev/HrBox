import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
	constructor() {}

	intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
		let token = '';

		request = request.clone({
			setHeaders: {
				Authorization: `Bearer ${token}` //,
				//'Access-Control-Allow-Origin' : '*'
			}
		});

		return next.handle(request);
	}
}
