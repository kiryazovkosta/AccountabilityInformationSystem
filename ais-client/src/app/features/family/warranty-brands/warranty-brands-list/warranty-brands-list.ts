import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { WarrantyBrandsService } from '../../warranty-records/services/warranty-brands.service';
import { ConfirmDialogService } from '../../../../common/confirm-dialog/confirm-dialog-service';

@Component({
  selector: 'app-warranty-brands-list',
  standalone: true,
  templateUrl: './warranty-brands-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WarrantyBrandsList {
  readonly #router = inject(Router);
  readonly #confirmDialogService = inject(ConfirmDialogService);
  protected readonly warrantyBrandsService = inject(WarrantyBrandsService);

  protected onCreate(): void {
    this.#router.navigate(['/family/warranty-brands/create']);
  }

  protected onView(id: string): void {
    this.#router.navigate(['/family/warranty-brands', id]);
  }

  protected async onDelete(id: string): Promise<void> {
    const confirmed = await this.#confirmDialogService.confirm({
      title: 'Delete warranty brand',
      message: 'This action cannot be undone.',
      confirmText: 'Delete',
      variant: 'danger',
    });
    if (!confirmed) {
      return;
    }
    this.warrantyBrandsService.delete(id).subscribe(() =>
      this.warrantyBrandsService.warrantyBrands.reload()
    );
  }
}
