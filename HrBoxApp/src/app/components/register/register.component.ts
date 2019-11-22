import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MustMatch } from '../../shared/helpers/must-match.validator';
import { UserService } from 'src/app/services/user.service';
import { GenericResponse } from 'src/app/shared/models/generic';
import { throwError } from 'rxjs';
import { faEnvelope, faLock, IconDefinition } from '@fortawesome/free-solid-svg-icons';

@Component({
	selector: 'app-register',
	templateUrl: './register.component.html',
	styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
	faEnvelope: IconDefinition = faEnvelope;
	faLock: IconDefinition = faLock;
	registerForm: FormGroup;
	submitting: boolean = false;
	submitted: boolean = false;
	success: boolean = false;
	submitMsg: string = '';

	get f() {
		return this.registerForm.controls as any;
	}

	constructor(private formBuilder: FormBuilder, private userService: UserService) {}

	// On submit of the register form create a user.
	registerUser(): void {
		//TODO: Replace bool with this.registerForm.valid:
		if (true) {
			this.resestSubmitStatus();
			this.userService.registerUser(this.registerForm.value).subscribe(
				(data: GenericResponse) => {
					if (data.Success === true) {
						console.log('Created User');
					} else {
						console.log('Failed to create user');
					}
				},
				error => {
					return throwError('Error creating user');
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
		this.registerForm = this.formBuilder.group(
			{
				FirstName: ['', Validators.required, Validators.minLength(2), Validators.maxLength(50)],
				LastName: ['', Validators.required, Validators.minLength(2), Validators.maxLength(80)],
				Email: ['', Validators.required, Validators.minLength(2), Validators.maxLength(300), Validators.email],
				Password: ['', Validators.required, Validators.minLength(6), Validators.maxLength(200)],
				ConfirmPassword: ['', Validators.required]
			},
			{ validator: MustMatch('password', 'confirmPassword') }
		);
	}
}
