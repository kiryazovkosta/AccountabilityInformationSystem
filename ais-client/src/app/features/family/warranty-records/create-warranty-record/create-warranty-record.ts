import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { form, FormField, schema, FormRoot, required, FieldTree } from '@angular/forms/signals';
import { FormError } from '../../../../shared/form-error/form-error';
import { HttpErrorService } from '../../../../services/shared/http-error.service';
import { WarrantyRecordsService } from '../services/warranty-records.service';
import { WarrantyBrandsService } from '../services/warranty-brands.service';
import { CreateWarrantyRecordRequest } from '../models/create-warranty-record.request';

const initialState: CreateWarrantyRecordRequest = {
  warrantyBrandId: '',
  model: '',
  purchaseDate: '',
  receipt: undefined,
  frontImage: undefined,
  backImage: undefined,
};

const createWarrantyRecordSchema = schema<CreateWarrantyRecordRequest>((path) => {
  required(path.warrantyBrandId, { message: 'Brand is required.' });
  required(path.model, { message: 'Model is required.' });
  required(path.purchaseDate, { message: 'Purchase date is required.' });
});

@Component({
  selector: 'app-create-warranty-record',
  standalone: true,
  imports: [FormField, FormRoot, FormError],
  templateUrl: './create-warranty-record.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [WarrantyBrandsService],
})
export class CreateWarrantyRecord {
  readonly #service = inject(WarrantyRecordsService);
  readonly #router = inject(Router);
  readonly #warrantyBrandsService = inject(WarrantyBrandsService);
  protected readonly httpErrorService = inject(HttpErrorService);

  protected readonly warrantyBrands = this.#warrantyBrandsService.warrantyBrands;

  protected readonly formData = signal<CreateWarrantyRecordRequest>(initialState);

  protected readonly warrantyForm = form(
    this.formData,
    createWarrantyRecordSchema,
    {
      submission: {
        action: async () => {
          this.httpErrorService.clear();
          try {
            console.log(this.warrantyForm().value());
            await firstValueFrom(this.#service.create(this.warrantyForm().value()));
            await this.#router.navigate(['/family/warranties']);
          } catch {
            // interceptor handles error display
          }
        }
      }
    }
  );

  protected onFileChange(event: Event, field: 'receipt' | 'frontImage' | 'backImage'): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.formData.update(prev => ({ ...prev, [field]: file }));
  }

  protected ariaInvalidState(field: FieldTree<unknown>): boolean | undefined {
    return field().touched() && !field().pending()
      ? field().errors().length > 0
      : undefined;
  }
}
