import { ChangeDetectionStrategy, Component, effect, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { WarrantyBrandsService } from '../../warranty-records/services/warranty-brands.service';

@Component({
  selector: 'app-warranty-brand',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [],
  templateUrl: './warranty-brand.html',
  styleUrl: './warranty-brand.css',
})
export class WarrantyBrand {
  readonly #router = inject(Router);
  protected readonly warrantyBrandsService = inject(WarrantyBrandsService);

  readonly id = input.required<string>();

  constructor() {
    effect(() => this.warrantyBrandsService.warrantyBrandId.set(this.id()));
  }

  protected onBack(): void {
    this.#router.navigate(['/family/warranty-brands']);
  }
}
