import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { IkunksService } from '../../services/measurement-flow/ikunks.service';

@Component({
  selector: 'app-features',
  imports: [],
  templateUrl: './features.html',
  styleUrl: './features.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Features {
  private readonly ikunksService = inject(IkunksService);

  readonly ikunks = this.ikunksService.ikunks;
}
