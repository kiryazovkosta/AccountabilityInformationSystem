import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { IkunksService } from '../services/ikunks.service';
import { IkunkItem } from "../ikunk-item/ikunk-item";

@Component({
  selector: 'app-ikunks-list',
  imports: [IkunkItem],
  templateUrl: './ikunks-list.html',
  styleUrl: './ikunks-list.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  providers: [IkunksService]
})
export class IkunksList {
  private readonly ikunksService = inject(IkunksService);

  readonly ikunks = this.ikunksService.ikunks;
}
