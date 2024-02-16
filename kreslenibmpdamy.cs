
public Bitmap Vizualizuj (int rozmerpx)
		{
			float dx = ((float)rozmerpx) / rozmer;
			Bitmap bmp = new Bitmap (rozmerpx, rozmerpx);
			using (Graphics g=Graphics.FromImage(bmp)) {
				for (int i=0; i<rozmer; i++)
					for (int j=0; j<rozmer; j++) {
						RectangleF r = new RectangleF (i * dx, j * dx, dx, dx);
						if ((i + j) % 2 == 0)
							g.FillRectangle (Brushes.Gray, r);
						else
							g.FillRectangle (Brushes.MistyRose, r);
					}
				for (int i=0; i<rozmer; i++)
				{
					g.DrawLine(Pens.Red,0,i*dx,rozmerpx,i*dx);
					g.DrawLine(Pens.Red,i*dx,0,i*dx,rozmerpx);
				}

				g.DrawLine(Pens.Red,0,rozmerpx-1,rozmerpx-1,rozmerpx-1);
				g.DrawLine(Pens.Red,rozmerpx-1,0,rozmerpx-1,rozmerpx-1);

			  foreach(Dama d in damy)
				{
					RectangleF r = new RectangleF (d.Pozice.X* dx, d.Pozice.Y * dx, dx, dx);
					d.Vizualizuj(g,r);
				}
			}
			return bmp;
