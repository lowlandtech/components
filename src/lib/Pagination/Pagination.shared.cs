namespace LowlandTech.Components.Pagination;

/// <summary>
/// Represents an item in the pagination (page number or ellipsis).
/// </summary>
public class PaginationPageItem
{
    /// <summary>
    /// The type of pagination item.
    /// </summary>
    public PaginationItemType Type { get; set; }
    
    /// <summary>
    /// The page number (for Page type items).
    /// </summary>
    public int Page { get; set; }
}

/// <summary>
/// Types of pagination items.
/// </summary>
public enum PaginationItemType
{
    /// <summary>
    /// A page number.
    /// </summary>
    Page,
    
    /// <summary>
    /// An ellipsis indicating skipped pages.
    /// </summary>
    Ellipsis
}

/// <summary>
/// Extends the auto-generated Pagination partial class.
/// </summary>
public partial class Pagination
{
    /// <summary>
    /// Generates the list of page items based on current settings.
    /// </summary>
    public List<PaginationPageItem> GetPageItems()
    {
        var items = new List<PaginationPageItem>();
        
        if (Count <= 0) return items;
        
        var totalPages = Count;
        var currentPage = Math.Clamp(Page, 1, totalPages);
        
        // Calculate the standard display window size
        // = start boundary + end boundary + siblings on each side + current page
        var windowSize = BoundaryCount * 2 + SiblingCount * 2 + 1;
        
        // If all pages fit in the window, show all without ellipsis
        if (totalPages <= windowSize)
        {
            for (var i = 1; i <= totalPages; i++)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
            }
            return items;
        }
        
        // When the current page is near the start, extend the display to avoid ellipsis
        // Show: 1, 2, ..., N where N = windowSize + boundary space
        var nearStartEnd = BoundaryCount + SiblingCount * 2 + 1;
        
        // When the current page is near the end, extend the display to avoid ellipsis
        // Show: N, ..., totalPages where N = totalPages - windowSize - boundary space
        var nearEndStart = totalPages - BoundaryCount - SiblingCount * 2;
        
        if (currentPage <= nearStartEnd)
        {
            // Near start: show continuous pages from 1 to nearStartEnd, then ellipsis, then end boundary
            for (var i = 1; i <= nearStartEnd; i++)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
            }
            
            // Add ellipsis if there's a gap before end boundary
            if (nearStartEnd < totalPages - BoundaryCount)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Ellipsis });
            }
            else if (nearStartEnd < totalPages)
            {
                // Fill the gap with pages
                for (var i = nearStartEnd + 1; i <= totalPages - BoundaryCount; i++)
                {
                    items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
                }
            }
            
            // Add end boundary pages
            for (var i = totalPages - BoundaryCount + 1; i <= totalPages; i++)
            {
                if (i > nearStartEnd)
                {
                    items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
                }
            }
        }
        else if (currentPage >= nearEndStart)
        {
            // Near end: show start boundary, then ellipsis, then continuous pages to end
            
            // Add start boundary pages
            for (var i = 1; i <= BoundaryCount; i++)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
            }
            
            // Add ellipsis if there's a gap after start boundary
            if (nearEndStart > BoundaryCount + 1)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Ellipsis });
            }
            else if (nearEndStart > BoundaryCount + 1)
            {
                // Fill the gap with pages
                for (var i = BoundaryCount + 1; i < nearEndStart; i++)
                {
                    items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
                }
            }
            
            // Add continuous pages to end
            for (var i = nearEndStart; i <= totalPages; i++)
            {
                if (i > BoundaryCount)
                {
                    items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
                }
            }
        }
        else
        {
            // Middle region: show both boundaries with ellipses and siblings around current
            
            // Add start boundary pages
            for (var i = 1; i <= BoundaryCount; i++)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
            }
            
            // Add first ellipsis
            items.Add(new PaginationPageItem { Type = PaginationItemType.Ellipsis });
            
            // Add sibling pages around current
            var siblingStart = currentPage - SiblingCount;
            var siblingEnd = currentPage + SiblingCount;
            for (var i = siblingStart; i <= siblingEnd; i++)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
            }
            
            // Add second ellipsis
            items.Add(new PaginationPageItem { Type = PaginationItemType.Ellipsis });
            
            // Add end boundary pages
            for (var i = totalPages - BoundaryCount + 1; i <= totalPages; i++)
            {
                items.Add(new PaginationPageItem { Type = PaginationItemType.Page, Page = i });
            }
        }
        
        return items;
    }
    
    /// <summary>
    /// Navigates to a specific page.
    /// </summary>
    public async Task GoToPage(int page)
    {
        if (page >= 1 && page <= Count && page != Page)
        {
            Page = page;
            await OnPageChange.InvokeAsync(page);
            StateHasChanged();
        }
    }
    
    /// <summary>
    /// Navigates to the previous page.
    /// </summary>
    public async Task GoToPrevious()
    {
        if (Page > 1)
        {
            await GoToPage(Page - 1);
        }
    }
    
    /// <summary>
    /// Navigates to the next page.
    /// </summary>
    public async Task GoToNext()
    {
        if (Page < Count)
        {
            await GoToPage(Page + 1);
        }
    }
}
