import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavToolbar } from './components/nav-toolbar/nav-toolbar';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavToolbar],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'telematics-ui';
}
