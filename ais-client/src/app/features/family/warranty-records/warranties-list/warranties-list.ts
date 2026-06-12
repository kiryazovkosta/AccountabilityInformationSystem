import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { WarrantyRecordsService } from '../services/warranty-records.service';
import { Pagination } from '../../../../shared/pagination/pagination';

@Component({
  selector: 'app-warranties-list',
  standalone: true,
  templateUrl: './warranties-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Pagination],
})
export class WarrantiesList {
  readonly #router = inject(Router);
  protected readonly warrantiesService = inject(WarrantyRecordsService);

  protected onCreate(): void {
    this.#router.navigate(['/family/warranty-records/create']);
  }

  protected onView(id: string): void {
    this.#router.navigate(['/family/warranty-records', id]);
  }
}
