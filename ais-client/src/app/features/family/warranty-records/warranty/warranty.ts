import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { httpResource } from '@angular/common/http';
import { Router } from '@angular/router';
import { WarrantyRecordResponse } from '../models/warranty-record.response';
import { apiUrl, Endpoints } from '../../../../core/http/endpoints-config';

@Component({
  selector: 'app-warranty',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe],
  templateUrl: './warranty.html',
  styleUrl: './warranty.css',
})
export class Warranty {
  readonly #router = inject(Router);

  readonly id = input.required<string>();

  /** Owned by this page, so opening another record never shows the previous one first. */
  protected readonly warrantyRecord = httpResource<WarrantyRecordResponse>(() => ({
    url: apiUrl(Endpoints.warrantyRecords, this.id()),
  }));

  protected onBack(): void {
    this.#router.navigate(['/family/warranties']);
  }
}
