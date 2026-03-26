import { Component, input } from '@angular/core';
import { IkunkResponse } from '../models/ikunks-paged.response';

@Component({
  selector: 'app-ikunk-item',
  imports: [],
  templateUrl: './ikunk-item.html',
  styleUrl: './ikunk-item.css',
})
export class IkunkItem {
  ikunk = input<IkunkResponse>()
}
