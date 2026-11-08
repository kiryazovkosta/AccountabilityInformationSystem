import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { Pagination } from './pagination';

function render(page: number, totalPages: number) {
  TestBed.configureTestingModule({ providers: [provideZonelessChangeDetection()] });
  const fixture = TestBed.createComponent(Pagination);
  fixture.componentRef.setInput('page', page);
  fixture.componentRef.setInput('totalPages', totalPages);
  fixture.detectChanges();
  return fixture;
}

describe('Pagination', () => {
  it('renders nothing for a single page', () => {
    const fixture = render(1, 1);
    expect(fixture.nativeElement.querySelector('nav')).toBeNull();
  });

  it('emits the clicked page and marks the current one', () => {
    const fixture = render(2, 5);
    const emitted: number[] = [];
    fixture.componentInstance.pageChange.subscribe(p => emitted.push(p));

    const buttons: HTMLButtonElement[] = Array.from(fixture.nativeElement.querySelectorAll('button'));
    expect(buttons.find(b => b.getAttribute('aria-current') === 'page')?.textContent?.trim()).toBe('2');

    buttons.find(b => b.textContent?.trim() === 'Next')!.click();
    buttons.find(b => b.textContent?.trim() === '4')!.click();
    expect(emitted).toEqual([3, 4]);
  });

  it('disables Previous on the first page', () => {
    const fixture = render(1, 3);
    const previous = Array.from<HTMLButtonElement>(fixture.nativeElement.querySelectorAll('button'))
      .find(b => b.textContent?.trim() === 'Previous')!;
    expect(previous.disabled).toBe(true);
  });
});
