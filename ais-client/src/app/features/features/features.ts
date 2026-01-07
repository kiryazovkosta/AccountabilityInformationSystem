import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';

@Component({
  selector: 'app-features',
  imports: [],
  templateUrl: './features.html',
  styleUrl: './features.css'
})
export class Features implements OnInit {
  httpClient = inject(HttpClient);

    ngOnInit(): void {
      this.httpClient.post('https://localhost:4001/api/identity/auth/login',
        {
          email: 'kosta@example.com',
          password: 'moc.elpmaxe@atsok'
        },
        {
          headers: {
            'Content-Type': 'application/json-patch+json; v=1',
            'Accept': '*/*'
          }
        }
      ).subscribe({
        next: (response) => console.log('Login response:', response),
        error: (error) => console.error('Login error:', error)
      });
  }
}
