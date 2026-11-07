import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { buildInfo } from '../../../environments/build-info';

@Component({
  selector: 'app-footer',
  imports: [DatePipe],
  templateUrl: './footer.html',
  styleUrl: './footer.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Footer {
  currentYear = signal<number>(new Date().getUTCFullYear());
  buildDate = signal<string>(buildInfo.buildDate);
}
