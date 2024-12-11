import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface Response {
  errorMessage: string,
  timeGenerated: Date,
  result: any
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  user: any = {}; // Datos del usuario
  course: any = {}; // Datos del curso
  loading: boolean = true; // Indicador de carga
  error: string | null = null; // Mensaje de error

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getContext();
  }

  getContext() {
    this.http.get<Response>('/api/lti', { withCredentials: true })
      .subscribe(
        (response: any) => {
          // Extraer datos del endpoint
          this.user = response.result.user || {};
          this.course = response.result.course || {};
          this.loading = false;
        },
        (err) => {
          console.error('Error al obtener los datos:', err);
          this.error = err?.error?.message || 'Error desconocido';
          this.loading = false;
        }
      );
  }
}
