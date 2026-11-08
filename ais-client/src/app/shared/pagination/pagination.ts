import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

/** Presentational pager: shows the current page and asks the parent to change it. */
@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Pagination {
  readonly page = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly totalCount = input<number | null>(null);
  /** How many page numbers to show around the current one. */
  readonly window = input(2);

  readonly pageChange = output<number>();

  protected readonly pages = computed(() => {
    const total = Math.max(this.totalPages(), 1);
    const current = Math.min(Math.max(this.page(), 1), total);
    const from = Math.max(1, current - this.window());
    const to = Math.min(total, current + this.window());
    return Array.from({ length: to - from + 1 }, (_, i) => from + i);
  });

  protected readonly hasPrevious = computed(() => this.page() > 1);
  protected readonly hasNext = computed(() => this.page() < this.totalPages());

  protected goTo(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.page()) return;
    this.pageChange.emit(page);
  }
}
