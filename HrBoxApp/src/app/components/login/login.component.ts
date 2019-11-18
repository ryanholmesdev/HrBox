import { Component, OnInit } from '@angular/core';

@Component({
	selector: 'app-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
	constructor() {}

	ngOnInit() {
		var elements = ['Hi', 'No'];
		elements.forEach(element => {
			if (element == 'Hi') {
				console.log('Hi Loaded');
			}
		});
	}
}
