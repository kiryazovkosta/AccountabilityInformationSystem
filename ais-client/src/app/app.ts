import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./layout/header/header";
import { Footer } from "./layout/footer/footer";
import { Toaster } from "./common/toast/toaster/toaster";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Footer, Toaster],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('ais-client');
}
