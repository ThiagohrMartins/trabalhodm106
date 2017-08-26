namespace TraballhoDM106.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alteracao_campos_produto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "name", c => c.String(nullable: false));
            AddColumn("dbo.Products", "description", c => c.String());
            AddColumn("dbo.Products", "color", c => c.String());
            AddColumn("dbo.Products", "model", c => c.String(nullable: false));
            AddColumn("dbo.Products", "code", c => c.String(nullable: false));
            AddColumn("dbo.Products", "price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "height", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "width", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "lenght", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "diameter", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Products", "nome");
            DropColumn("dbo.Products", "descricao");
            DropColumn("dbo.Products", "cor");
            DropColumn("dbo.Products", "modelo");
            DropColumn("dbo.Products", "codigo");
            DropColumn("dbo.Products", "preco");
            DropColumn("dbo.Products", "peso");
            DropColumn("dbo.Products", "altura");
            DropColumn("dbo.Products", "largura");
            DropColumn("dbo.Products", "comprimento");
            DropColumn("dbo.Products", "diametro");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "diametro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "comprimento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "largura", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "altura", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "peso", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "preco", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "codigo", c => c.String(nullable: false));
            AddColumn("dbo.Products", "modelo", c => c.String(nullable: false));
            AddColumn("dbo.Products", "cor", c => c.String());
            AddColumn("dbo.Products", "descricao", c => c.String());
            AddColumn("dbo.Products", "nome", c => c.String(nullable: false));
            DropColumn("dbo.Products", "diameter");
            DropColumn("dbo.Products", "lenght");
            DropColumn("dbo.Products", "width");
            DropColumn("dbo.Products", "height");
            DropColumn("dbo.Products", "weight");
            DropColumn("dbo.Products", "price");
            DropColumn("dbo.Products", "code");
            DropColumn("dbo.Products", "model");
            DropColumn("dbo.Products", "color");
            DropColumn("dbo.Products", "description");
            DropColumn("dbo.Products", "name");
        }
    }
}
