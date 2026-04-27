import { Component, input } from '@angular/core';
import { FieldTree } from '@angular/forms/signals';

@Component({
  selector: 'app-form-error',
  imports: [],
  styleUrl: './form-error.css',
  template: `
    @let state = fieldRef()();
    @if (state.touched() && state.invalid()) {
    <div class="form-error-message">
      @for (error of state.errors(); track error.message) {
        <p>{{ error.message }}</p>
      }
    </div>
    }
  `
})
export class FormError<T> {
  readonly fieldRef = input.required<FieldTree<T>>();
}
