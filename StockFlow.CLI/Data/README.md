# Sample inventory data

These JSON files are **intentional sample data** for the StockFlow demo. They let a fresh clone explore products, stock movements, low-stock reports, and the inventory summary immediately.

- `products.json` — active products with prices, quantities, and soft-delete status.
- `stock-movements.json` — matching stock In/Out history.

The records are fictional and contain no personal information. StockFlow reads these files at startup and saves changes back to them, so you can experiment freely.

To start over, delete either file and launch the app again. It will create an empty file on the next save. Note that a missing file loads as an empty list; a corrupted file stops startup with an error instead of silently replacing your data.
