import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { WarrantyRecordsService } from '../services/warranty-records.service';
import { Pagination } from '../../../../shared/pagination/pagination';
import { ConfirmDialogService } from '../../../../common/confirm-dialog/confirm-dialog-service';

@Component({
  selector: 'app-warranties-list',
  standalone: true,
  templateUrl: './warranties-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Pagination],
})
export class WarrantiesList {
  readonly #router = inject(Router);
  readonly #confirmDialogService = inject(ConfirmDialogService);
  protected readonly warrantiesService = inject(WarrantyRecordsService);

  protected onCreate(): void {
    this.#router.navigate(['/family/warranty-records/create']);
  }

  protected onView(id: string): void {
    this.#router.navigate(['/family/warranty-records', id]);
  }

  protected async onDelete(id: string): Promise<void> {
    const confirmed = await this.#confirmDialogService.confirm({
      title: 'Delete warranty record',
      message: 'This will also remove any attached files. This action cannot be undone.',
      confirmText: 'Delete',
      variant: 'danger',
    });
    if (!confirmed) {
      return;
    }
    this.warrantiesService.delete(id).subscribe(() =>
      this.warrantiesService.warrantyRecords.reload()
    );
  }
}
