import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { GenericResponse } from 'src/app/shared/models/generic';
import { throwError } from 'rxjs';
import { faEnvelope, faLock, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { AuthService } from 'src/app/services/auth.service';
import { first } from 'rxjs/operators';
import { LoginResponse } from 'src/app/shared/models/auth';
@Component({
	selector: 'app-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
	faEnvelope: IconDefinition = faEnvelope;
	faLock: IconDefinition = faLock;
	loginForm: FormGroup;
	submitting: boolean = false;
	submitted: boolean = false;
	success: boolean = false;
	submitMsg: string = '';

	get f() {
		return this.loginForm.controls as any;
	}

	constructor(private formBuilder: FormBuilder, private authService: AuthService) {}

	// On submit of the register form create a user.
	loginUser(): void {
		this.submitted = true;
		//TODO: Replace bool with this.registerForm.valid:
		if (this.loginForm.valid) {
			this.resestSubmitStatus();
			this.authService
				.login(this.loginForm.value)
				.pipe(first())
				.subscribe(
					(data: LoginResponse) => {
						if (data.Response.Success) {
							console.log('LOGGED IN');
							//this.router.navigate(['/profile']);
						} else {
							this.submitMsg = data.Response.Msg ? data.Response.Msg : 'Unable to login';
							this.success = false;
						}
						this.submitting = false;
					},
					error => {
						console.log(error);
						this.submitting = false;
						this.success = false;
						this.submitMsg = 'Error occured';
						this.submitted = true;
					}
				);
		}
	}

	// Resets the submitting form variables.
	private resestSubmitStatus(): void {
		this.submitting = true;
		this.submitted = false;
		this.success = false;
		this.submitMsg = '';
	}

	ngOnInit() {
		this.loginForm = this.formBuilder.group({
			Email: ['', [Validators.required, Validators.email]],
			Password: ['', [Validators.required]]
		});
	}
}
