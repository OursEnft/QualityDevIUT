using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json; // Pour la sérialisation JSON

// Classe de base Media qui représente un média générique
public class Media
{
    public string Titre { get; set; }
    public int NumeroDeReference { get; set; }
    public int NombreExemplairesDisponibles { get; set; }

    // Constructeur de la classe Media
    public Media(string titre, int numeroDeReference, int nombreExemplairesDisponibles)
    {
        Titre = titre;
        NumeroDeReference = numeroDeReference;
        NombreExemplairesDisponibles = nombreExemplairesDisponibles;
    }

    // Méthode pour afficher les informations d'un média
    public virtual void AfficherInfos()
    {
        Console.WriteLine($"Titre: {Titre}, Réf: {NumeroDeReference}, Disponibles: {NombreExemplairesDisponibles}");
    }

    // Surcharge de l'opérateur + pour ajouter des exemplaires
    public static Media operator +(Media media, int nombre)
    {
        media.NombreExemplairesDisponibles += nombre;
        return media;
    }

    // Surcharge de l'opérateur - pour retirer des exemplaires
    public static Media operator -(Media media, int nombre)
    {
        if (media.NombreExemplairesDisponibles >= nombre)
            media.NombreExemplairesDisponibles -= nombre;
        else
            throw new InvalidOperationException("Exemplaires insuffisants.");
        return media;
    }
}

// Classe Livre qui hérite de Media
public class Livre : Media
{
    public string Auteur { get; set; }

    // Constructeur de la classe Livre
    public Livre(string titre, int numero, int nbExemplaires, string auteur)
        : base(titre, numero, nbExemplaires)
    {
        Auteur = auteur;
    }

    // Affichage spécifique pour un Livre
    public override void AfficherInfos()
    {
        base.AfficherInfos(); // Appel de la méthode de base
        Console.WriteLine($"Auteur: {Auteur}");
    }
}

// Classe CD qui hérite de Media
public class CD : Media
{
    public string Artiste { get; set; }

    public CD(string titre, int numero, int nbExemplaires, string artiste)
        : base(titre, numero, nbExemplaires)
    {
        Artiste = artiste;
    }

    public override void AfficherInfos()
    {
        base.AfficherInfos();
        Console.WriteLine($"Artiste: {Artiste}");
    }
}

// Classe DVD qui hérite de Media
public class DVD : Media
{
    public string Duree { get; set; }

    public DVD(string titre, int numero, int nbExemplaires, string duree)
        : base(titre, numero, nbExemplaires)
    {
        Duree = duree;
    }

    public override void AfficherInfos()
    {
        base.AfficherInfos();
        Console.WriteLine($"Durée: {Duree}");
    }
}

// Classe Library qui gère une collection de médias
public class Library
{
    private List<Media> medias = new List<Media>(); // Liste qui contient les médias

    // Accéder à un média par son numéro de référence via un indexeur
    public Media this[int numero] => medias.Find(m => m.NumeroDeReference == numero);

    // Ajouter un média à la bibliothèque
    public void AjouterMedia(Media media) => medias.Add(media);

    // Retirer un média de la bibliothèque
    public void RetirerMedia(Media media) => medias.Remove(media);

    // Emprunter un média (décrémenter le nombre d'exemplaires)
    public void EmprunterMedia(int numero)
    {
        Media media = this[numero];
        if (media != null && media.NombreExemplairesDisponibles > 0)
            media.NombreExemplairesDisponibles--;
        else
            throw new InvalidOperationException("Média non disponible.");
    }

    // Retourner un média (incrémenter le nombre d'exemplaires)
    public void RetournerMedia(int numero)
    {
        Media media = this[numero];
        if (media != null)
            media.NombreExemplairesDisponibles++;
    }

    // Rechercher un média par titre ou auteur
    public List<Media> RechercherMedia(string critere)
    {
        return medias.FindAll(m => m.Titre.Contains(critere) ||
                                   (m is Livre && ((Livre)m).Auteur.Contains(critere)));
    }

    // Afficher des statistiques simples de la bibliothèque
    public void AfficherStatistiques()
    {
        int totalMedias = medias.Count;
        int totalDisponibles = medias.Sum(m => m.NombreExemplairesDisponibles);
        Console.WriteLine($"Total des médias: {totalMedias}, Exemplaires disponibles: {totalDisponibles}");
    }

    // Sauvegarder la bibliothèque en JSON
    public void Sauvegarder(string chemin)
    {
        var json = JsonConvert.SerializeObject(medias, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(chemin, json);
    }

    // Charger la bibliothèque depuis un fichier JSON
    public void Charger(string chemin)
    {
        if (File.Exists(chemin))
            medias = JsonConvert.DeserializeObject<List<Media>>(File.ReadAllText(chemin));
    }
}

// Programme principal qui teste les fonctionnalités
class Program
{
    static void Main(string[] args)
    {
        Library library = new Library();

        // Ajout de quelques médias à la bibliothèque
        library.AjouterMedia(new Livre("Le Petit Prince", 1, 5, "Antoine de Saint-Exupéry"));
        library.AjouterMedia(new CD("Thriller", 2, 3, "Michael Jackson"));
        library.AjouterMedia(new DVD("Inception", 3, 2, "2h28"));

        // Tentative d'emprunter un média
        try
        {
            library.EmprunterMedia(1); // Emprunte "Le Petit Prince"
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); // Afficher l'erreur si problème
        }

        // Afficher les informations des médias
        Console.WriteLine("\nMédias dans la bibliothèque:");
        foreach (var media in library.RechercherMedia("")) // Recherche vide = tout afficher
        {
            media.AfficherInfos();
            Console.WriteLine();
        }

        // Sauvegarde de la bibliothèque dans un fichier JSON
        library.Sauvegarder("bibliotheque.json");

        // Charger les médias depuis le fichier et afficher les statistiques
        library.Charger("bibliotheque.json");
        Console.WriteLine("\nStatistiques de la bibliothèque:");
        library.AfficherStatistiques();
    }
}
