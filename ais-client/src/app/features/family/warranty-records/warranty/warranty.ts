import { ChangeDetectionStrategy, Component, effect, inject, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { WarrantyRecordsService } from '../services/warranty-records.service';

@Component({
  selector: 'app-warranty',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe],
  templateUrl: './warranty.html',
  styleUrl: './warranty.css',
})
export class Warranty {
  readonly #router = inject(Router);
  protected readonly warrantiesService = inject(WarrantyRecordsService);

  readonly id = input.required<string>();

  constructor() {
    effect(() => this.warrantiesService.warrantyId.set(this.id()));
  }

  protected onBack(): void {
    this.#router.navigate(['/family/warranties']);
  }
}
