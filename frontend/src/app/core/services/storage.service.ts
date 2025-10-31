import { Injectable } from '@angular/core';
import { StorageKeys } from '@shared/constants/storage-keys'; // <-- Importe as suas chaves

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  private storage: Storage = localStorage;

  constructor() {}

  public saveItem(key: StorageKeys, value: any): void {
    const stringValue = JSON.stringify(value);
    this.storage.setItem(key, stringValue);
  }

  public getItem<T>(key: StorageKeys): T | null {
    const item = this.storage.getItem(key);

    if (item) {
      try {
        return JSON.parse(item) as T;
      } catch (e) {
        console.error('Erro ao fazer parse do item do storage:', e);
        this.storage.removeItem(key);
        return null;
      }
    }
    return null;
  }
  public removeItem(key: StorageKeys): void {
    this.storage.removeItem(key);
  }
  public clearAll(): void {
    this.storage.clear();
  }
}
