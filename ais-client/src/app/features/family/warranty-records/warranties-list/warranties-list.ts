import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-warranties-list',
  standalone: true,
  templateUrl: './warranties-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WarrantiesList {
  readonly #router = inject(Router);

  protected onCreate(): void {
    this.#router.navigate(['/family/warranty-records/create']);
  }
}
