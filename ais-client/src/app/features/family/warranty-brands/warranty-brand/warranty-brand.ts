import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { httpResource } from '@angular/common/http';
import { Router } from '@angular/router';
import { WarrantyBrandResponse } from '../models/warranty-brand.response';
import { apiUrl, Endpoints } from '../../../../core/http/endpoints-config';

@Component({
  selector: 'app-warranty-brand',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],
  templateUrl: './warranty-brand.html',
  styleUrl: './warranty-brand.css',
})
export class WarrantyBrand {
  readonly #router = inject(Router);

  readonly id = input.required<string>();

  /** Owned by this page, so opening another brand never shows the previous one first. */
  protected readonly warrantyBrand = httpResource<WarrantyBrandResponse>(() => ({
    url: apiUrl(Endpoints.warrantyBrands, this.id()),
  }));

  protected onBack(): void {
    this.#router.navigate(['/family/warranty-brands']);
  }
}
