import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ToastService } from '../toast-service';

@Component({
  selector: 'app-toaster',
  imports: [],
  templateUrl: './toaster.html',
  styleUrl: './toaster.css',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Toaster {
  protected toastService = inject(ToastService);
}
