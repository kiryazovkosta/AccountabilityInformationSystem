import { ChangeDetectionStrategy, Component, effect, inject, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { WarrantyRecordsService } from '../services/warranty-records.service';
import { Pagination } from '../../../../shared/pagination/pagination';
import { ConfirmDialogService } from '../../../../shared/confirm-dialog/confirm-dialog-service';

@Component({
  selector: 'app-warranties-list',
  standalone: true,
  templateUrl: './warranties-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Pagination, DatePipe],
})
export class WarrantiesList {
  readonly #router = inject(Router);
  readonly #confirmDialogService = inject(ConfirmDialogService);
  readonly #route = inject(ActivatedRoute);
  protected readonly warrantiesService = inject(WarrantyRecordsService);

  /** `?page=N` from the URL (bound by withComponentInputBinding), so reload and back keep the page. */
  readonly pageParam = input<string | undefined>(undefined, { alias: 'page' });

  constructor() {
    effect(() => {
      const page = Number(this.pageParam() ?? 1);
      this.warrantiesService.page.set(Number.isInteger(page) && page > 0 ? page : 1);
    });
  }

  protected onPageChange(page: number): void {
    this.#router.navigate([], {
      relativeTo: this.#route,
      queryParams: { page },
      queryParamsHandling: 'merge',
    });
  }

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
