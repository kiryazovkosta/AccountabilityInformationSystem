import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { form, FormField, schema, FormRoot, required, FieldTree } from '@angular/forms/signals';
import { FormError } from '../../../../shared/form-error/form-error';
import { HttpErrorService } from '../../../../services/shared/http-error.service';
import { ToastService } from '../../../../common/toast/toast-service';
import { WarrantyBrandsService } from '../../warranty-records/services/warranty-brands.service';
import { CreateWarrantyBrandRequest } from '../../warranty-records/models/create-warranty-brand.request';

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
  readonly #toastService = inject(ToastService);
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
              // The name is already taken (often by a previous attempt that actually
              // succeeded) — the data the user wanted now exists, so treat this as
              // a success rather than leaving them on a form that looks broken.
              this.httpErrorService.clear();
              this.#toastService.show(`A brand named "${name}" already exists.`, 'info');
            } else {
              // interceptor handles error display
              return;
            }
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
