import { Injectable, signal } from '@angular/core';
import { Toast } from './toast.model';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  toasts = signal<Toast[]>([]);

  show(message: string, type: Toast['type'] = 'info') {
    const id = crypto.randomUUID();
    this.toasts.update((toast) => [...toast, {id, message, type}]);

    setTimeout(() => this.remove(id), 5000);
  }

  remove(id: string) {
    this.toasts.update((toast) => toast.filter((t) => t.id !== id));
  }
}
