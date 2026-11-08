import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { form, FormField, schema, FormRoot, required, FieldTree } from '@angular/forms/signals';
import { FormError } from '../../../../shared/form-error/form-error';
import { HttpErrorService } from '../../../../core/http/http-error.service';
import { WarrantyBrandsService } from '../services/warranty-brands.service';
import { CreateWarrantyBrandRequest } from '../models/create-warranty-brand.request';

const initialState: CreateWarrantyBrandRequest = {
  name: '',
  logo: '',
};

const createWarrantyBrandSchema = schema<CreateWarrantyBrandRequest>((path) => {
  required(path.name, { message: 'Name is required.' });
});

@Component({
  selector: 'app-create-warranty-brand',
  standalone: true,
  imports: [FormField, FormRoot, FormError],
  templateUrl: './create-warranty-brand.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateWarrantyBrand {
  readonly #service = inject(WarrantyBrandsService);
  readonly #router = inject(Router);
  protected readonly httpErrorService = inject(HttpErrorService);

  protected readonly formData = signal<CreateWarrantyBrandRequest>(initialState);

  protected readonly warrantyBrandForm = form(
    this.formData,
    createWarrantyBrandSchema,
    {
      submission: {
        action: async () => {
          this.httpErrorService.clear();
          const name = this.warrantyBrandForm().value().name;
          try {
            await firstValueFrom(this.#service.create(this.warrantyBrandForm().value()));
          } catch (error) {
            if (error instanceof HttpErrorResponse && error.status === 409) {
              // Keep the user on the form: nothing was created, the name is taken.
              this.httpErrorService.currentError.set({
                status: 409,
                detail: `A brand named "${name}" already exists. Choose another name.`,
              });
            }
            // Other errors are recorded by the error interceptor and shown inline.
            return;
          }
          this.#service.warrantyBrands.reload();
          await this.#router.navigate(['/family/warranty-brands']);
        }
      }
    }
  );

  protected ariaInvalidState(field: FieldTree<unknown>): boolean | undefined {
    return field().touched() && !field().pending()
      ? field().errors().length > 0
      : undefined;
  }
}
