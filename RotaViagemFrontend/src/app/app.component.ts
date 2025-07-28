import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

interface Rota {
  id: BigInteger;
  origem: string;
  destino: string;
  valor: number;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ReactiveFormsModule, HttpClientModule, CommonModule, RouterModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  rotas: Rota[] = [];
  rotaForm: FormGroup;
  resultadoConsulta: string = '';
  private apiUrl = 'https://localhost:5000/api/Rotas';

  constructor(private http: HttpClient, private fb: FormBuilder) {
    this.rotaForm = this.fb.group({
      origem: [''],
      destino: [''],
      valor: [0],
    });
  }

  ngOnInit(): void {
    this.listarRotas();
  }

  listarRotas() {
    this.http.get<Rota[]>(this.apiUrl).subscribe(
      (data) => {
        //console.log('Rotas recebidas do backend:', data);
        this.rotas = data;
      },
      (error) => {
        //console.error('Erro ao buscar rotas:', error);
      }
    );
  }

  adicionarRota() {
    const rota = this.rotaForm.value;
    this.http.post<Rota>(this.apiUrl, rota).subscribe(() => {
      this.listarRotas();
      this.rotaForm.reset();
    });
  }

  consultarMelhorRota(origem: string, destino: string) {
    this.http
      .get(`${this.apiUrl}/melhor-rota?origem=${origem}&destino=${destino}`, {
        responseType: 'text',
      })
      .subscribe((data) => (this.resultadoConsulta = data));
  }

  deletarRota(rota: Rota) {
    this.http
      .delete(`${this.apiUrl}/${rota.id}`)
      .subscribe(() => this.listarRotas());
  }
}
